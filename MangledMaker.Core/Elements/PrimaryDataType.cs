namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class PrimaryDataType : ComplexElement
    {
        public enum PrimaryTypes
        {
            FunctionPointer,
            Pointer,
            IndirectBasic,
            Reference,
            VolatileReference,
            Basic
        }

        private BasicDataType? basicDataType;
        private BasicDataType BasicDataTypeSafe => this.basicDataType ??= new(this, this.SuperType);
        private DataIndirectType? dataIndirectType;
        private DataIndirectType DataIndirectTypeSafe => this.dataIndirectType ??= new(this, this.SuperType, (IndirectType)'\0', new(), false);
        private FunctionIndirectType? functionIndirectType;
        private FunctionIndirectType FunctionIndirectTypeSafe => this.functionIndirectType ??= new(this, this.SuperType);
        private bool isMissing;
        private PtrRefDataType? ptrRefDataType;
        private PtrRefDataType PtrRefDataTypeSafe => this.ptrRefDataType ??= new(this, this.SuperType, true);
        private ReferenceType? referenceType;
        private ReferenceType ReferenceTypeSafe => this.referenceType ??= new(this, this.SuperType, new());

        private PrimaryTypes type;

        public PrimaryDataType(ComplexElement parent, DecoratedName superType)
            : base(parent) =>
            this.SuperType = superType;

        public unsafe PrimaryDataType(ComplexElement parent, ref char* pSource,
                                      DecoratedName superType)
            : this(parent, superType) =>
            this.Parse(ref pSource);

        [Input]
        public DecoratedName SuperType { get; set; }

        [Setting]
        public PrimaryTypes Type
        {
            get => this.type;
            set
            {
                this.type = value;
                this.isMissing = false;
            }
        }

        [Child]
        public FunctionIndirectType? FunctionIndirectType => this.type == PrimaryTypes.FunctionPointer ? this.functionIndirectType : null;

        [Child]
        public PtrRefDataType? PtrRefDataType => this.type == PrimaryTypes.Pointer ? this.ptrRefDataType : null;

        [Child]
        public BasicDataType? BasicDataType =>
            this.type is PrimaryTypes.IndirectBasic or PrimaryTypes.Basic
                ? this.basicDataType
                : null;

        [Child]
        public DataIndirectType? DataIndirectType => this.type == PrimaryTypes.IndirectBasic ? this.dataIndirectType : null;

        [Child]
        public ReferenceType? ReferenceType =>
            this.type is PrimaryTypes.Reference or PrimaryTypes.VolatileReference
                ? this.referenceType
                : null;

        protected override DecoratedName GenerateName()
        {
            if (this.isMissing) 
                return new DecoratedName(this, NodeStatus.Truncated) + this.SuperType;
            switch (this.type)
            {
                case PrimaryTypes.FunctionPointer:
                    this.FunctionIndirectTypeSafe.SuperType = this.SuperType;
                    return this.FunctionIndirectTypeSafe.Name;
                case PrimaryTypes.Pointer:
                    this.PtrRefDataTypeSafe.SuperType = this.SuperType;
                    return this.PtrRefDataTypeSafe.Name;
                case PrimaryTypes.IndirectBasic:
                    this.DataIndirectTypeSafe.SuperType = this.SuperType;
                    this.BasicDataTypeSafe.SuperType = this.DataIndirectTypeSafe.Name;
                    return this.BasicDataTypeSafe.Name;
                case PrimaryTypes.Reference:
                    DecoratedName super = new(this, this.SuperType) {IsPointerReference = true};
                    this.ReferenceTypeSafe.CvType = new(this);
                    this.ReferenceTypeSafe.SuperType = super;
                    return this.ReferenceTypeSafe.Name;
                case PrimaryTypes.VolatileReference:
                    DecoratedName cvType = new(this, "volatile");
                    if (!this.SuperType.IsEmpty)
                        cvType.Append(' ');
                    DecoratedName superv = new(this, this.SuperType) {IsPointerReference = true};
                    this.ReferenceTypeSafe.CvType = cvType;
                    this.ReferenceTypeSafe.SuperType = superv;
                    return this.ReferenceTypeSafe.Name;
                case PrimaryTypes.Basic:
                    this.BasicDataTypeSafe.SuperType = this.SuperType;
                    return this.BasicDataTypeSafe.Name;
            }
            return new DecoratedName(this, NodeStatus.Truncated) + this.SuperType;
        }

        private unsafe void Parse(ref char* pSource)
        {
            switch (*pSource)
            {
                case '\0':
                    this.isMissing = true;
                    return;
                case '$':
                    if (pSource[1] != '$')
                    {
                        if (pSource[1] == '\0')
                            this.isMissing = true;
                        else
                            this.IsInvalid = true;
                        return;
                    }
                    pSource += 2;
                    switch (*pSource)
                    {
                        case '\0':
                            this.isMissing = true;
                            return;
                        case 'A':
                            this.type = PrimaryTypes.FunctionPointer;
                            pSource++;
                            this.functionIndirectType = new(this, ref pSource, this.SuperType);
                            return;
                        case 'B':
                            this.type = PrimaryTypes.Pointer;
                            pSource++;
                            this.ptrRefDataType = new(this, ref pSource,
                                                                     this.SuperType, true);
                            return;
                        case 'C':
                            this.type = PrimaryTypes.IndirectBasic;
                            pSource++;
                            this.dataIndirectType = new(this, ref pSource,
                                                                         this.SuperType, (IndirectType)'\0', new(),
                                                                         false);
                            this.basicDataType = new(this, ref pSource,
                                                                   this.dataIndirectType.Name);
                            return;
                        default:
                            this.IsInvalid = true;
                            return;
                    }
                case 'A':
                    this.type = PrimaryTypes.Reference;
                    break;
                case 'B':
                    this.type = PrimaryTypes.VolatileReference;
                    break;
                default:
                    this.type = PrimaryTypes.Basic;
                    this.basicDataType = new(this, ref pSource, this.SuperType);
                    return;
            }
            DecoratedName super = new(this.SuperType);
            pSource++;
            super.IsPointerReference = true;
            this.referenceType = new(this, ref pSource, new(), super);
        }

        protected override DecoratedName GenerateCode()
        {
            if (this.isMissing) 
                return new(this, '\0');
            switch (this.type)
            {
                case PrimaryTypes.FunctionPointer:
                    this.FunctionIndirectTypeSafe.SuperType = this.SuperType;
                    return new DecoratedName(this, "$$A") + this.FunctionIndirectTypeSafe.Code;
                case PrimaryTypes.Pointer:
                    this.PtrRefDataTypeSafe.SuperType = this.SuperType;
                    return new DecoratedName(this, "$$B") + this.PtrRefDataTypeSafe.Code;
                case PrimaryTypes.IndirectBasic:
                    this.DataIndirectTypeSafe.SuperType = this.SuperType;
                    this.BasicDataTypeSafe.SuperType = this.DataIndirectTypeSafe.Name;
                    return new DecoratedName(this, "$$C") + this.DataIndirectTypeSafe.Code + this.BasicDataTypeSafe.Code;
                case PrimaryTypes.Reference:
                    DecoratedName super = new(this, this.SuperType) {IsPointerReference = true};
                    this.ReferenceTypeSafe.CvType = new DecoratedName(this);
                    this.ReferenceTypeSafe.SuperType = super;
                    return new DecoratedName(this, 'A') + this.ReferenceTypeSafe.Code;
                case PrimaryTypes.VolatileReference:
                    DecoratedName cvType = new(this, "volatile");
                    if (!this.SuperType.IsEmpty)
                        cvType.Append(' ');
                    DecoratedName superv = new(this, this.SuperType) {IsPointerReference = true};
                    this.ReferenceTypeSafe.CvType = cvType;
                    this.ReferenceTypeSafe.SuperType = superv;
                    return new DecoratedName(this, 'B') + this.ReferenceTypeSafe.Code;
                case PrimaryTypes.Basic:
                    this.BasicDataTypeSafe.SuperType = this.SuperType;
                    return this.BasicDataTypeSafe.Code;
            }
            return new(this, '\0');
        }
    }
}