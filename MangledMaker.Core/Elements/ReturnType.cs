namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class ReturnType : ComplexElement
    {
        public ReturnType(ComplexElement parent, DecoratedName declarator)
            : base(parent)
        {
            this.Declarator = declarator;
            this.NoReturnType = true;
        }

        public unsafe ReturnType(ComplexElement parent, ref char* pSource,
                                 DecoratedName declarator)
            : this(parent, declarator) =>
            this.Parse(ref pSource);

        [Input]
        public DecoratedName Declarator { get; set; }

        [Setting]
        public bool NoReturnType { get; set; }

        private DataType? dataType;
        [Child] public DataType DataType => this.dataType ??= new(this, this.Declarator);

        protected override DecoratedName GenerateName()
        {
            if (this.NoReturnType)
                return DecoratedName.CreateReference(this, this.Declarator);
            this.DataType.Declarator = this.Declarator;
            return this.DataType.Name;
        }

        private unsafe void Parse(ref char* pSource)
        {
            if (*pSource == '@')
            {
                pSource++;
                this.NoReturnType = true;
            }
            else
            {
                this.NoReturnType = false;
                this.dataType = new(this, ref pSource, this.Declarator);
            }
        }

        protected override DecoratedName GenerateCode()
        {
            if (this.NoReturnType)
                return new(this, '@');
            this.DataType.Declarator = this.Declarator;
            return this.DataType.Code;
        }
    }
}