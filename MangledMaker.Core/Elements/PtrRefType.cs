namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class PtrRefType : ComplexElement
    {
        private DataIndirectType dataIndirectType;
        private FunctionIndirectType functionIndirectType;
        private bool isFunctionPointer;
        private bool isMissing;
        private PtrRefDataType ptrRefDataType;

        public PtrRefType(ComplexElement parent, DecoratedName cvType,
                          DecoratedName superType, IndirectType prType)
            : base(parent)
        {
            this.CvType = cvType;
            this.SuperType = superType;
            this.PrType = prType;
        }

        public unsafe PtrRefType(ComplexElement parent, ref char* pSource,
                                 DecoratedName cvType, DecoratedName superType, IndirectType prType)
            : this(parent, cvType, superType, prType)
        {
            this.Parse(ref pSource);
        }

        [Input]
        public DecoratedName SuperType { get; set; }

        [Input]
        public IndirectType PrType { get; set; }

        [Input]
        public DecoratedName CvType { get; set; }

        [Setting]
        public bool IsFunctionPointer
        {
            get { return this.isFunctionPointer; }
            set
            {
                this.isFunctionPointer = value;
                this.isMissing = false;
            }
        }

        [Child]
        public FunctionIndirectType FunctionIndirectType
        {
            get { return this.isFunctionPointer ? this.functionIndirectType : null; }
        }

        [Child]
        public DataIndirectType DataIndirectType
        {
            get { return this.isFunctionPointer ? null : this.dataIndirectType; }
        }

        [Child]
        public PtrRefDataType PtrRefDataType
        {
            get { return this.isFunctionPointer ? null : this.ptrRefDataType; }
        }

        protected override void CreateEmptyElements()
        {
            if (this.functionIndirectType == null)
                this.functionIndirectType =
                    new FunctionIndirectType(this, new DecoratedName());
            if (this.dataIndirectType == null)
                this.dataIndirectType =
                    new DataIndirectType(this, this.SuperType, this.PrType, this.CvType, false);
            if (this.ptrRefDataType == null)
                this.ptrRefDataType =
                    new PtrRefDataType(this, new DecoratedName(), this.PrType == IndirectType.Pointer);
        }

        protected override DecoratedName GenerateName()
        {
            if (!this.isMissing)
            {
                if (this.isFunctionPointer)
                {
                    var modifiers = new DecoratedName(this, IndirectionToChar(this.PrType));
                    if (!this.CvType.IsEmpty && (this.SuperType.IsEmpty || !this.SuperType.IsPointerReference))
                        modifiers.Append(this.CvType);
                    if (!this.SuperType.IsEmpty)
                        modifiers.Append(this.SuperType);
                    this.functionIndirectType.SuperType = modifiers;
                    return this.functionIndirectType.Name;
                }
                this.dataIndirectType.SuperType = this.SuperType;
                this.dataIndirectType.PrType = this.PrType;
                this.dataIndirectType.CvType = this.CvType;
                this.ptrRefDataType.SuperType = this.dataIndirectType.Name;
                this.ptrRefDataType.IsPtr = this.PrType == IndirectType.Pointer;
                return this.ptrRefDataType.Name;
            }
            var missing = new DecoratedName(this, NodeStatus.Truncated);
            missing.Append(IndirectionToChar(this.PrType));
            if (!this.CvType.IsEmpty)
                missing.Append(this.CvType);
            if (this.SuperType.IsEmpty) 
                return missing;
            if (!this.CvType.IsEmpty)
                missing.Append(' ');
            missing.Append(this.SuperType);
            return missing;
        }

        private unsafe void Parse(ref char* pSource)
        {
            if (*pSource == '\0')
            {
                this.isMissing = true;
                return;
            }

            this.isMissing = false;
            if (*pSource >= '6' && *pSource <= '9' || *pSource == '_')
            {
                this.isFunctionPointer = true;
                var modifiers = new DecoratedName(IndirectionToChar(this.PrType));
                if (!this.CvType.IsEmpty && (this.SuperType.IsEmpty || !this.SuperType.IsPointerReference))
                    modifiers.Append(this.CvType);
                if (!this.SuperType.IsEmpty)
                    modifiers.Append(this.SuperType);
                this.functionIndirectType = new FunctionIndirectType(this, ref pSource, modifiers);
                return;
            }
            this.isFunctionPointer = false;
            this.dataIndirectType = new DataIndirectType(this, ref pSource, this.SuperType,
                this.PrType, this.CvType, false);
            this.ptrRefDataType = new PtrRefDataType(this, ref pSource, this.dataIndirectType.Name,
                this.PrType == IndirectType.Pointer);
        }

        protected override DecoratedName GenerateCode()
        {
            if (this.isMissing) 
                return new DecoratedName(this, '\0');
            if (this.isFunctionPointer)
            {
                var modifiers = new DecoratedName(this, IndirectionToChar(this.PrType));
                if (!this.CvType.IsEmpty && (this.SuperType.IsEmpty || !this.SuperType.IsPointerReference))
                    modifiers.Append(this.CvType);
                if (!this.SuperType.IsEmpty)
                    modifiers.Append(this.SuperType);
                this.functionIndirectType.SuperType = modifiers;
                return this.functionIndirectType.Code;
            }
            this.dataIndirectType.SuperType = this.SuperType;
            this.dataIndirectType.PrType = this.PrType;
            this.dataIndirectType.CvType = this.CvType;
            this.ptrRefDataType.SuperType = this.dataIndirectType.Name;
            this.ptrRefDataType.IsPtr = this.PrType == IndirectType.Pointer;
            return this.dataIndirectType.Code + this.ptrRefDataType.Code;
        }
    }
}