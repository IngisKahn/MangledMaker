namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class CustomName : ComplexElement
    {
        public CustomName(ComplexElement parent, string prefix)
            : base(parent)
        {
            this.Prefix = prefix;

            this.Index = new SignedDimension(this, false);
        }

        public unsafe CustomName(ComplexElement parent, ref char* pSource, string prefix)
            : base(parent)
        {
            this.Prefix = prefix;
            this.Parse(ref pSource);
        }

        [Child]
        public SignedDimension Index { get; private set; }

        [Input]
        public string Prefix { get; set; }

        protected override DecoratedName GenerateName()
        {
            string param;
            if (this.UnDecorator.HaveTemplateParameters
                && (param = this.UnDecorator.GetParameter((int)this.Index.Dimension.Value)) != null)
                return new DecoratedName(this, param);
            return new DecoratedName(this, '`') + this.Prefix + new DecoratedName(this, this.Index.Name) + '\'';
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.Index = new SignedDimension(this, ref pSource);
        }

        protected override DecoratedName GenerateCode()
        {
            return this.Index.Code;
        }
    }
}