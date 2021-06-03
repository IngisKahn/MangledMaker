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

        private DataIndirectType dataIndirectType;

        private bool isMissing;

        private DataTypeMode mode;

        private PrimaryDataType primaryDataType;

        public DataType(ComplexElement parent, DecoratedName declarator)
            : base(parent)
        {
            this.Declarator = declarator;
        }

        public unsafe DataType(ComplexElement parent, ref char* pSource, DecoratedName declarator)
            : this(parent, declarator)
        {
            this.Parse(ref pSource);
        }

        [Input]
        public DecoratedName Declarator { get; set; }

        [Setting]
        public DataTypeMode Mode
        {
            get { return this.mode; }
            set
            {
                this.mode = value;
                this.isMissing = false;
            }
        }

        [Child]
        public DataIndirectType DataIndirectType
        {
            get { return this.mode == DataTypeMode.Indirect ? this.dataIndirectType : null; }
        }

        [Child]
        public PrimaryDataType PrimaryDataType
        {
            get { return this.mode == DataTypeMode.Void ? null : this.primaryDataType; }
        }

        protected override void CreateEmptyElements()
        {
            if (this.dataIndirectType == null) this.dataIndirectType = new DataIndirectType(this);
            if (this.primaryDataType == null)
                this.primaryDataType = new PrimaryDataType(this, new DecoratedName(this));
        }

        protected override DecoratedName GenerateName()
        {
            var superType = DecoratedName.CreateReference(this, this.Declarator);
            if (this.isMissing)
                return new DecoratedName(this, NodeStatus.Truncated) + superType;

            switch (this.mode)
            {
                case DataTypeMode.Indirect:
                    superType.Assign(this.dataIndirectType.Name);
                    this.primaryDataType.SuperType = superType;
                    return this.primaryDataType.Name;
                case DataTypeMode.Void:
                    if (superType.IsEmpty)
                        return new DecoratedName(this, "void");
                    return new DecoratedName(this, "void ") + superType;
                default:
                    this.primaryDataType.SuperType = superType;
                    return this.primaryDataType.Name;
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
                    this.dataIndirectType = new DataIndirectType(this, ref pSource, superType, (IndirectType)'\0',
                                                                 new DecoratedName(), false);
                    superType.Assign(this.dataIndirectType.Name);
                    this.primaryDataType = new PrimaryDataType(this, ref pSource, superType);
                    break;
                case 'X':
                    this.mode = DataTypeMode.Void;
                    pSource++;
                    break;
                default:
                    this.mode = DataTypeMode.Direct;
                    this.primaryDataType = new PrimaryDataType(this, ref pSource, superType);
                    break;
            }
        }

        protected override DecoratedName GenerateCode()
        {
            switch (this.mode)
            {
                case DataTypeMode.Indirect:
                    return new DecoratedName(this, '?') + this.dataIndirectType.Code + this.primaryDataType.Code;
                case DataTypeMode.Void:
                    return new DecoratedName(this, 'X');
                default:
                    return this.primaryDataType.Code;
            }
        }
    }
}