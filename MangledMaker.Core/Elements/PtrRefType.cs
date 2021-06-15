namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class PtrRefType : ComplexElement
    {
        private DataIndirectType? dataIndirectType;
        private DataIndirectType DataIndirectTypeSafe => this.dataIndirectType ??= new(this, this.SuperType, this.PrType, this.CvType, false);
        private FunctionIndirectType? functionIndirectType;
        private FunctionIndirectType FunctionIndirectTypeSafe => this.functionIndirectType ??= new(this, new());
        private bool isFunctionPointer;
        private bool isMissing;
        private PtrRefDataType? ptrRefDataType;
        private PtrRefDataType PtrRefDataTypeSafe => this.ptrRefDataType ??= new(this, new(), this.PrType == IndirectType.Pointer);

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
            : this(parent, cvType, superType, prType) =>
            this.Parse(ref pSource);

        [Input]
        public DecoratedName SuperType { get; set; }

        [Input]
        public IndirectType PrType { get; set; }

        [Input]
        public DecoratedName CvType { get; set; }

        [Setting]
        public bool IsFunctionPointer
        {
            get => this.isFunctionPointer;
            set
            {
                this.isFunctionPointer = value;
                this.isMissing = false;
            }
        }
        [Child]
        public FunctionIndirectType? FunctionIndirectType => this.isFunctionPointer ? this.functionIndirectType : null;

        [Child]
        public DataIndirectType? DataIndirectType => this.isFunctionPointer ? null : this.dataIndirectType;

        [Child]
        public PtrRefDataType? PtrRefDataType => this.isFunctionPointer ? null : this.ptrRefDataType;

        protected override DecoratedName GenerateName()
        {
            if (!this.isMissing)
            {
                if (this.isFunctionPointer)
                {
                    DecoratedName modifiers = new(this, Element.IndirectionToChar(this.PrType));
                    if (!this.CvType.IsEmpty && (this.SuperType.IsEmpty || !this.SuperType.IsPointerReference))
                        modifiers.Append(this.CvType);
                    if (!this.SuperType.IsEmpty)
                        modifiers.Append(this.SuperType);
                    this.FunctionIndirectTypeSafe.SuperType = modifiers;
                    return this.FunctionIndirectTypeSafe.Name;
                }
                this.DataIndirectTypeSafe.SuperType = this.SuperType;
                this.DataIndirectTypeSafe.PrType = this.PrType;
                this.DataIndirectTypeSafe.CvType = this.CvType;
                this.PtrRefDataTypeSafe.SuperType = this.DataIndirectTypeSafe.Name;
                this.PtrRefDataTypeSafe.IsPtr = this.PrType == IndirectType.Pointer;
                return this.PtrRefDataTypeSafe.Name;
            }
            DecoratedName missing = new(this, NodeStatus.Truncated);
            missing.Append(Element.IndirectionToChar(this.PrType));
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
                DecoratedName modifiers = new(Element.IndirectionToChar(this.PrType));
                if (!this.CvType.IsEmpty && (this.SuperType.IsEmpty || !this.SuperType.IsPointerReference))
                    modifiers.Append(this.CvType);
                if (!this.SuperType.IsEmpty)
                    modifiers.Append(this.SuperType);
                this.functionIndirectType = new(this, ref pSource, modifiers);
                return;
            }
            this.isFunctionPointer = false;
            this.dataIndirectType = new(this, ref pSource, this.SuperType,
                this.PrType, this.CvType, false);
            this.ptrRefDataType = new(this, ref pSource, this.dataIndirectType.Name,
                this.PrType == IndirectType.Pointer);
        }

        protected override DecoratedName GenerateCode()
        {
            if (this.isMissing) 
                return new(this, '\0');
            if (this.isFunctionPointer)
            {
                DecoratedName modifiers = new(this, Element.IndirectionToChar(this.PrType));
                if (!this.CvType.IsEmpty && (this.SuperType.IsEmpty || !this.SuperType.IsPointerReference))
                    modifiers.Append(this.CvType);
                if (!this.SuperType.IsEmpty)
                    modifiers.Append(this.SuperType);
                this.FunctionIndirectTypeSafe.SuperType = modifiers;
                return this.FunctionIndirectTypeSafe.Code;
            }
            this.DataIndirectTypeSafe.SuperType = this.SuperType;
            this.DataIndirectTypeSafe.PrType = this.PrType;
            this.DataIndirectTypeSafe.CvType = this.CvType;
            this.PtrRefDataTypeSafe.SuperType = this.DataIndirectTypeSafe.Name;
            this.PtrRefDataTypeSafe.IsPtr = this.PrType == IndirectType.Pointer;
            return this.DataIndirectTypeSafe.Code + this.PtrRefDataTypeSafe.Code;
        }
    }
}