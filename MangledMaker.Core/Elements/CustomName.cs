namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class CustomName : ComplexElement
    {
        public CustomName(ComplexElement parent, string prefix)
            : base(parent)
        {
            this.Prefix = prefix;

            this.Index = new(this, false);
        }

        public unsafe CustomName(ComplexElement parent, ref char* pSource, string prefix)
            : base(parent)
        {
            this.Prefix = prefix;
            this.Index = new(this, ref pSource);
        }

        [Child]
        public SignedDimension Index { get; }

        [Input]
        public string Prefix { get; set; }

        protected override DecoratedName GenerateName()
        {
            string? param;
            return this.UnDecorator.HaveTemplateParameters
                   && (param = this.UnDecorator.GetParameter((int) this.Index.Dimension.Value)) != null
                ? new(this, param)
                : new DecoratedName(this, '`') + this.Prefix + new DecoratedName(this, this.Index.Name) + '\'';
        }

        protected override DecoratedName GenerateCode() => this.Index.Code;
    }
}