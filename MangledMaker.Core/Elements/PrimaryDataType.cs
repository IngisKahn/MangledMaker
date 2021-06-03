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

        private BasicDataType basicDataType;
        private DataIndirectType dataIndirectType;
        private FunctionIndirectType functionIndirectType;
        private bool isMissing;
        private PtrRefDataType ptrRefDataType;
        private ReferenceType referenceType;

        private PrimaryTypes type;

        public PrimaryDataType(ComplexElement parent, DecoratedName superType)
            : base(parent)
        {
            this.SuperType = superType;
        }

        public unsafe PrimaryDataType(ComplexElement parent, ref char* pSource,
                                      DecoratedName superType)
            : this(parent, superType)
        {
            this.Parse(ref pSource);
        }

        [Input]
        public DecoratedName SuperType { get; set; }

        [Setting]
        public PrimaryTypes Type
        {
            get { return this.type; }
            set
            {
                this.type = value;
                this.isMissing = false;
            }
        }

        [Child]
        public FunctionIndirectType FunctionIndirectType
        {
            get { return this.type == PrimaryTypes.FunctionPointer ? this.functionIndirectType : null; }
        }

        [Child]
        public PtrRefDataType PtrRefDataType
        {
            get { return this.type == PrimaryTypes.Pointer ? this.ptrRefDataType : null; }
        }

        [Child]
        public BasicDataType BasicDataType
        {
            get
            {
                return this.type == PrimaryTypes.IndirectBasic || this.type == PrimaryTypes.Basic
                           ? this.basicDataType
                           : null;
            }
        }

        [Child]
        public DataIndirectType DataIndirectType
        {
            get { return this.type == PrimaryTypes.IndirectBasic ? this.dataIndirectType : null; }
        }

        [Child]
        public ReferenceType ReferenceType
        {
            get
            {
                return this.type == PrimaryTypes.Reference || this.type == PrimaryTypes.VolatileReference
                           ? this.referenceType
                           : null;
            }
        }

        protected override void CreateEmptyElements()
        {
            if (this.functionIndirectType == null)
                this.functionIndirectType =
                    new FunctionIndirectType(this, this.SuperType);
            if (this.ptrRefDataType == null)
                this.ptrRefDataType =
                    new PtrRefDataType(this, this.SuperType, true);
            if (this.dataIndirectType == null)
                this.dataIndirectType =
                    new DataIndirectType(this, this.SuperType, (IndirectType)'\0', new DecoratedName(), false);
            if (this.basicDataType == null)
                this.basicDataType =
                    new BasicDataType(this, this.SuperType);
            if (this.referenceType != null) return;
// ReSharper disable once ObjectCreationAsStatement
            new DecoratedName(this.SuperType) {IsPointerReference = true};
            this.referenceType =
                new ReferenceType(this, this.SuperType, new DecoratedName());
        }

        protected override DecoratedName GenerateName()
        {
            if (this.isMissing) 
                return new DecoratedName(this, NodeStatus.Truncated) + this.SuperType;
            switch (this.type)
            {
                case PrimaryTypes.FunctionPointer:
                    this.functionIndirectType.SuperType = this.SuperType;
                    return this.functionIndirectType.Name;
                case PrimaryTypes.Pointer:
                    this.ptrRefDataType.SuperType = this.SuperType;
                    return this.ptrRefDataType.Name;
                case PrimaryTypes.IndirectBasic:
                    this.dataIndirectType.SuperType = this.SuperType;
                    this.basicDataType.SuperType = this.dataIndirectType.Name;
                    return this.basicDataType.Name;
                case PrimaryTypes.Reference:
                    var super = new DecoratedName(this, this.SuperType) {IsPointerReference = true};
                    this.referenceType.CvType = new DecoratedName(this);
                    this.referenceType.SuperType = super;
                    return this.referenceType.Name;
                case PrimaryTypes.VolatileReference:
                    var cvType = new DecoratedName(this, "volatile");
                    if (!this.SuperType.IsEmpty)
                        cvType.Append(' ');
                    var superv = new DecoratedName(this, this.SuperType) {IsPointerReference = true};
                    this.referenceType.CvType = cvType;
                    this.referenceType.SuperType = superv;
                    return this.referenceType.Name;
                case PrimaryTypes.Basic:
                    this.basicDataType.SuperType = this.SuperType;
                    return this.basicDataType.Name;
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
                            this.functionIndirectType = new FunctionIndirectType(this, ref pSource, this.SuperType);
                            return;
                        case 'B':
                            this.type = PrimaryTypes.Pointer;
                            pSource++;
                            this.ptrRefDataType = new PtrRefDataType(this, ref pSource,
                                                                     this.SuperType, true);
                            return;
                        case 'C':
                            this.type = PrimaryTypes.IndirectBasic;
                            pSource++;
                            this.dataIndirectType = new DataIndirectType(this, ref pSource,
                                                                         this.SuperType, (IndirectType)'\0', new DecoratedName(),
                                                                         false);
                            this.basicDataType = new BasicDataType(this, ref pSource,
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
                    this.basicDataType = new BasicDataType(this, ref pSource, this.SuperType);
                    return;
            }
            var super = new DecoratedName(this.SuperType);
            pSource++;
            super.IsPointerReference = true;
            this.referenceType = new ReferenceType(this, ref pSource, new DecoratedName(), super);
        }

        protected override DecoratedName GenerateCode()
        {
            if (this.isMissing) 
                return new DecoratedName(this, '\0');
            switch (this.type)
            {
                case PrimaryTypes.FunctionPointer:
                    this.functionIndirectType.SuperType = this.SuperType;
                    return new DecoratedName(this, "$$A") + this.functionIndirectType.Code;
                case PrimaryTypes.Pointer:
                    this.ptrRefDataType.SuperType = this.SuperType;
                    return new DecoratedName(this, "$$B") + this.ptrRefDataType.Code;
                case PrimaryTypes.IndirectBasic:
                    this.dataIndirectType.SuperType = this.SuperType;
                    this.basicDataType.SuperType = this.dataIndirectType.Name;
                    return new DecoratedName(this, "$$C") + this.dataIndirectType.Code + this.basicDataType.Code;
                case PrimaryTypes.Reference:
                    var super = new DecoratedName(this, this.SuperType) {IsPointerReference = true};
                    this.referenceType.CvType = new DecoratedName(this);
                    this.referenceType.SuperType = super;
                    return new DecoratedName(this, 'A') + this.referenceType.Code;
                case PrimaryTypes.VolatileReference:
                    var cvType = new DecoratedName(this, "volatile");
                    if (!this.SuperType.IsEmpty)
                        cvType.Append(' ');
                    var superv = new DecoratedName(this, this.SuperType) {IsPointerReference = true};
                    this.referenceType.CvType = cvType;
                    this.referenceType.SuperType = superv;
                    return new DecoratedName(this, 'B') + this.referenceType.Code;
                case PrimaryTypes.Basic:
                    this.basicDataType.SuperType = this.SuperType;
                    return this.basicDataType.Code;
            }
            return new DecoratedName(this, '\0');
        }
    }
}