namespace MangledMaker.Core.Elements
{
    using System;
    using System.Collections.Generic;
    using Attributes;

    public sealed class ArrayType : ComplexElement, ISpawnsChildren
    {
        //int dimensionCount;

        //[Setting]
        //public int DimensionCount
        //{
        //    get { return dimensionCount; }
        //    set 
        //    {
        //        isMissing = false;
        //        int count = Math.Max(value, 0);
        //        if (count > dimensionCount)
        //        {
        //            count -= dimensionCount;
        //            while (count-- != 0)
        //                dimensions.Add(new Dimension(this, 1, false));
        //            dimensionCount = value;
        //        }
        //        else if (count != dimensionCount)
        //        {
        //            dimensions.RemoveRange(count, dimensionCount - count);
        //            dimensionCount = value;
        //        }
        //    }
        //}

        private readonly List<Dimension> dimensions = new();
        private bool isMissing;

        private PrimaryDataType? primaryDataType;

        private BasicDataType? unknownSizedType;

        public ArrayType(ComplexElement parent, DecoratedName superType)
            : base(parent) =>
            this.SuperType = superType;

        public unsafe ArrayType(ComplexElement parent, ref char* pSource,
                                DecoratedName superType)
            : this(parent, superType) =>
            this.Parse(ref pSource);

        [Child]
        public BasicDataType? UnknownSizedType => this.dimensions.Count < 1 ? this.unknownSizedType ??= new(this, this.SuperType) : null;

        [Child]
        public List<Dimension>? Dimensions => this.dimensions.Count > 0 ? this.dimensions : null;

        [Child]
        public PrimaryDataType? PrimaryDataType => this.dimensions.Count > 0 ? this.primaryDataType ??= new(this, this.SuperType) : null;

        [Input]
        public DecoratedName SuperType { get; set; }

        public Element CreateChild() => new Dimension(this, 0);

        private static unsafe int GetNumberOfDimensions(ref char* pSource)
        {
            switch (*pSource)
            {
                case '\0':
                    return 0;
                case >= '0' and <= '9':
                    return *pSource - '/';
            }

            var result = 0;
            for (; *pSource != '@'; pSource++)
                switch (*pSource)
                {
                    case '\0':
                        return 0;
                    case < 'A':
                    case > 'P':
                        return -1;
                    default:
                        result <<= 4;
                        result += *pSource - 'A';
                        break;
                }

            return *pSource++ == '@' ? result : -1;
        }

        protected override DecoratedName GenerateName()
        {
            var dimensionCount = this.dimensions.Count;
            if (this.isMissing)
            {
                var missing = new DecoratedName(this, '[') + NodeStatus.Truncated + ']';
                if (!this.SuperType.IsEmpty)
                    missing.Prepend('(' + new DecoratedName(this, this.SuperType) + ')');
                (this.unknownSizedType ??= new(this, this.SuperType)).SuperType = missing;
                return this.unknownSizedType.Name;
            }

            if (dimensionCount == 0)
            {
                (this.unknownSizedType ??= new(this, this.SuperType)).SuperType = new DecoratedName(this, '[') + NodeStatus.Truncated + ']';
                return this.unknownSizedType.Name;
            }

            DecoratedName indices = new();
            if (this.SuperType.IsArray)
                indices.Append("[]");

            foreach (var d in this.dimensions)
                indices.Append('[' + new DecoratedName(this, d.Name) + ']');

            if (!this.SuperType.IsEmpty)
                if (this.SuperType.IsArray)
                    indices.Assign(this.SuperType + indices);
                else
                    indices.Assign('(' + new DecoratedName(this, this.SuperType) + ')' + indices);
            (this.primaryDataType ??= new(this, this.SuperType)).SuperType = indices;
            var result = this.primaryDataType.Name;
            result.IsArray = true;
            return result;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.dimensions.Clear();

            if (*pSource == '\0')
            {
                this.isMissing = true;
                this.unknownSizedType = new(this, ref pSource, this.SuperType);
                return;
            }

            this.isMissing = false;
            var count = Math.Max(ArrayType.GetNumberOfDimensions(ref pSource), 0);
            //dimensionCount = count;
            if (count == 0)
                this.unknownSizedType = new(this, ref pSource, this.SuperType);
            else
            {
                while (count-- != 0)
                    this.dimensions.Add(new(this, ref pSource, false));

                this.primaryDataType = new(this, ref pSource, this.SuperType);
            }
        }

        protected override DecoratedName GenerateCode()
        {
            var dimensionCount = this.dimensions.Count;
            if (this.isMissing) 
                return new(this, '\0');
            DecoratedName code = new(this);

            if (dimensionCount is > 0 and < 10)
                code.Assign((char)(dimensionCount + '0'));
            else
            {
                var count = dimensionCount;
                while (count != 0)
                {
                    code.Append((char)((count & 0xF) + 'A'));
                    count >>= 4;
                }
                code.Append('@');
            }

            if (dimensionCount == 0 && this.unknownSizedType is not null)
                code.Append(this.unknownSizedType.Code);
            else
            {
                foreach (var d in this.dimensions)
                    code.Append(d.Code);
                if (this.primaryDataType is not null)
                    code.Append(this.primaryDataType.Code);
            }
            return code;
        }
    }
}