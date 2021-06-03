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

        private readonly List<Dimension> dimensions = new List<Dimension>();
        private bool isMissing;

        private PrimaryDataType primaryDataType;

        private BasicDataType unknownSizedType;

        public ArrayType(ComplexElement parent, DecoratedName superType)
            : base(parent)
        {
            this.SuperType = superType;
        }

        public unsafe ArrayType(ComplexElement parent, ref char* pSource,
                                DecoratedName superType)
            : this(parent, superType)
        {
            this.Parse(ref pSource);
        }

        [Child]
        public BasicDataType UnknownSizedType
        {
            get { return this.dimensions.Count < 1 ? this.unknownSizedType : null; }
        }

        [Child]
        public List<Dimension> Dimensions
        {
            get { return this.dimensions.Count > 0 ? this.dimensions : null; }
        }

        [Child]
        public PrimaryDataType PrimaryDataType
        {
            get { return this.dimensions.Count > 0 ? this.primaryDataType : null; }
        }

        [Input]
        public DecoratedName SuperType { get; set; }

        #region ISpawnsChildren Members

        public Element CreateChild()
        {
            return new Dimension(this, 0);
        }

        #endregion

        private static unsafe int GetNumberOfDimensions(ref char* pSource)
        {
            if (*pSource == '\0')
                return 0;
            if ((*pSource >= '0') && (*pSource <= '9'))
                return *pSource - '/';
            var result = 0;
            for (; *pSource != '@'; pSource++)
            {
                if (*pSource == '\0')
                    return 0;
                if ((*pSource < 'A') || (*pSource > 'P'))
                    return -1;
                result <<= 4;
                result += *pSource - 'A';
            }

            return *pSource++ == '@' ? result : -1;
        }

        protected override void CreateEmptyElements()
        {
            if (this.unknownSizedType == null)
                this.unknownSizedType = new BasicDataType(this, this.SuperType);
            if (this.primaryDataType == null)
                this.primaryDataType = new PrimaryDataType(this, this.SuperType);
        }

        protected override DecoratedName GenerateName()
        {
            var dimensionCount = this.dimensions.Count;
            if (this.isMissing)
            {
                var missing = new DecoratedName(this, '[') + NodeStatus.Truncated + ']';
                if (!this.SuperType.IsEmpty)
                    missing.Prepend('(' + new DecoratedName(this, this.SuperType) + ')');
                this.unknownSizedType.SuperType = missing;
                return this.unknownSizedType.Name;
            }

            if (dimensionCount == 0)
            {
                this.unknownSizedType.SuperType = new DecoratedName(this, '[') + NodeStatus.Truncated + ']';
                return this.unknownSizedType.Name;
            }

            var indices = new DecoratedName();
            if (this.SuperType.IsArray)
                indices.Append("[]");

            foreach (var d in this.dimensions)
                indices.Append('[' + new DecoratedName(this, d.Name) + ']');

            if (!this.SuperType.IsEmpty)
                if (this.SuperType.IsArray)
                    indices.Assign(this.SuperType + indices);
                else
                    indices.Assign('(' + new DecoratedName(this, this.SuperType) + ')' + indices);
            this.primaryDataType.SuperType = indices;
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
                this.unknownSizedType = new BasicDataType(this, ref pSource, this.SuperType);
                return;
            }

            this.isMissing = false;
            var count = Math.Max(GetNumberOfDimensions(ref pSource), 0);
            //dimensionCount = count;
            if (count == 0)
                this.unknownSizedType = new BasicDataType(this, ref pSource, this.SuperType);
            else
            {
                while (count-- != 0)
                    this.dimensions.Add(new Dimension(this, ref pSource, false));

                this.primaryDataType = new PrimaryDataType(this, ref pSource, this.SuperType);
            }
        }

        protected override DecoratedName GenerateCode()
        {
            var dimensionCount = this.dimensions.Count;
            if (this.isMissing) 
                return new DecoratedName(this, '\0');
            var code = new DecoratedName(this);

            if (dimensionCount > 0 && dimensionCount < 10)
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

            if (dimensionCount == 0)
                code.Append(this.unknownSizedType.Code);
            else
            {
                foreach (var d in this.dimensions)
                    code.Append(d.Code);
                code.Append(this.primaryDataType.Code);
            }
            return code;
        }
    }
}