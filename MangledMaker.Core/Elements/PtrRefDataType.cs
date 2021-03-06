namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class PtrRefDataType : ComplexElement
    {
        private bool isArray;
        private bool isMissing;
        private bool isVoid;

        public PtrRefDataType(ComplexElement parent, DecoratedName superType,
                              bool isPtr)
            : base(parent)
        {
            this.SuperType = superType;
            this.IsPtr = isPtr;
        }

        public unsafe PtrRefDataType(ComplexElement parent, ref char* pSource,
                                     DecoratedName superType, bool isPtr)
            : this(parent, superType, isPtr)
        {
            this.Parse(ref pSource);
        }

        [Input]
        public DecoratedName SuperType { get; set; }

        [Input]
        public bool IsPtr { get; set; }

        [Setting]
        public bool? IsVoid
        {
            get => this.IsPtr ? this.isVoid : null;
            set
            {
                if (value != null) this.isVoid = (bool)value;
                this.isMissing = false;
            }
        }

        [Setting]
        public bool IsArray
        {
            get => this.isArray;
            set
            {
                this.isArray = value;
                this.isMissing = false;
            }
        }

        private ArrayType? arrayType;
        [Child]
        public ArrayType ArrayType
        {
            get => this.arrayType ??= new(this, this.SuperType);
            private set => this.arrayType = value;
        }

        private BasicDataType? basicDataType;
        [Child]
        public BasicDataType BasicDataType
        {
            get => this.basicDataType ??= new(this, this.SuperType);
            private set => this.basicDataType = value;
        }

        protected override DecoratedName GenerateName()
        {
            if (this.isMissing)
                return new DecoratedName(this, NodeStatus.Truncated) + this.SuperType;
            if (this.IsPtr && this.isVoid)
                return this.SuperType.IsEmpty
                    ? new(this, "void")
                    : new DecoratedName(this, "void ") + this.SuperType;

            if (this.isArray)
            {
                this.ArrayType.SuperType = this.SuperType;
                return this.ArrayType.Name;
            }

            this.BasicDataType.SuperType = this.SuperType;
            DecoratedName result = new(this, this.BasicDataType.Name);
            if (this.SuperType.IsComArray)
                result.Prepend("cli::array<");
            else if (this.SuperType.IsPinnedPointer)
                result.Prepend("cli::pin_ptr<");
            return result;
        }

        private unsafe void Parse(ref char* pSource)
        {
            if (*pSource == '\0')
            {
                this.isMissing = true;
                return;
            }

            this.isMissing = false;
            if (this.IsPtr && (*pSource == 'X'))
            {
                this.isVoid = true;
                pSource++;
                return;
            }
            this.isVoid = false;

            if (*pSource == 'Y')
            {
                this.isArray = true;
                pSource++;
                this.ArrayType = new(this, ref pSource, this.SuperType);
                return;
            }
            this.isArray = false;
            this.BasicDataType = new(this, ref pSource, this.SuperType);
        }

        protected override DecoratedName GenerateCode()
        {
            if (this.isMissing)
                return new(this, '\0');
            if (this.IsPtr && this.isVoid)
                return new(this, 'X');
            if (this.isArray)
            {
                this.ArrayType.SuperType = this.SuperType;
                return new DecoratedName(this, 'Y') + this.ArrayType.Code;
            }
            this.BasicDataType.SuperType = this.SuperType;
            return this.BasicDataType.Code;
        }
    }
}