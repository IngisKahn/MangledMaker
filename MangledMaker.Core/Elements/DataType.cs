namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class DataType : ComplexElement
    {
        public enum DataTypeMode
        {
            //Missing,
            Indirect,
            Void,
            Direct
        }

        private DataIndirectType? dataIndirectType;

        private bool isMissing;

        private DataTypeMode mode;

        private PrimaryDataType? primaryDataType;

        public DataType(ComplexElement parent, DecoratedName? declarator)
            : base(parent) =>
            this.Declarator = declarator;

        public unsafe DataType(ComplexElement parent, ref char* pSource, DecoratedName? declarator)
            : this(parent, declarator) =>
            this.Parse(ref pSource);

        [Input]
        public DecoratedName? Declarator { get; set; }

        [Setting]
        public DataTypeMode Mode
        {
            get => this.mode;
            set
            {
                this.mode = value;
                this.isMissing = false;
            }
        }

        private DataIndirectType DataIndirectTypeSafe => this.dataIndirectType ??= new(this);
        private PrimaryDataType PrimaryDataTypeSafe => this.primaryDataType ??= new(this, new());

        [Child]
        public DataIndirectType? DataIndirectType => this.mode == DataTypeMode.Indirect ? this.DataIndirectTypeSafe : null;

        [Child]
        public PrimaryDataType? PrimaryDataType => this.mode == DataTypeMode.Void ? null : this.PrimaryDataTypeSafe;

        protected override DecoratedName GenerateName()
        {
            var superType = DecoratedName.CreateReference(this, this.Declarator);
            if (this.isMissing)
                return new DecoratedName(this, NodeStatus.Truncated) + superType;

            switch (this.mode)
            {
                case DataTypeMode.Indirect:
                    superType.Assign(this.DataIndirectTypeSafe.Name);
                    this.PrimaryDataTypeSafe.SuperType = superType;
                    return this.PrimaryDataTypeSafe.Name;
                case DataTypeMode.Void:
                    if (superType.IsEmpty)
                        return new(this, "void");
                    return new DecoratedName(this, "void ") + superType;
                default:
                    this.PrimaryDataTypeSafe.SuperType = superType;
                    return this.PrimaryDataTypeSafe.Name;
            }
        }

        private unsafe void Parse(ref char* pSource)
        {
            var superType = DecoratedName.CreateReference(this.Declarator);

            this.isMissing = false;
            switch (*pSource)
            {
                case '\0':
                    this.isMissing = true;
                    break;
                case '?':
                    this.mode = DataTypeMode.Indirect;
                    pSource++;
                    this.dataIndirectType = new(this, ref pSource, superType, (IndirectType)'\0',
                                                                 new(), false);
                    superType.Assign(this.dataIndirectType.Name);
                    this.primaryDataType = new(this, ref pSource, superType);
                    break;
                case 'X':
                    this.mode = DataTypeMode.Void;
                    pSource++;
                    break;
                default:
                    this.mode = DataTypeMode.Direct;
                    this.primaryDataType = new(this, ref pSource, superType);
                    break;
            }
        }

        protected override DecoratedName GenerateCode() =>
            this.mode switch
            {
                DataTypeMode.Indirect => new DecoratedName(this, '?') + this.DataIndirectTypeSafe.Code +
                                         this.PrimaryDataTypeSafe.Code,
                DataTypeMode.Void => new(this, 'X'),
                _ => this.PrimaryDataTypeSafe.Code
            };
    }
}