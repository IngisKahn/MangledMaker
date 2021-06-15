namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class SignedDimension : Element
    {
        private bool negative;

        public SignedDimension(Element parent, bool isNegative)
            : base(parent)
        {
            this.negative = isNegative;
            this.Dimension = new(this, 0);
        }

        public unsafe SignedDimension(Element parent, ref char* pSource) : base(parent) => this.Parse(ref pSource);

        public bool FixedSign { get; set; }

        [Setting]
        public bool? Negative
        {
            get => this.FixedSign ? null : this.negative;
            set { if (value != null) this.negative = (bool)value; }
        }

        private Dimension? dimension;
        [Child]
        public Dimension Dimension { get => this.dimension ??= new(this); private set => this.dimension = value; }

        protected override DecoratedName GenerateName() => this.negative ? '-' + new DecoratedName(this, this.Dimension.Name) : this.Dimension.Name;

        private unsafe void Parse(ref char* pSource)
        {
            if (*pSource == '\0')
                this.IsTruncated = true;
            else
            {
                if (*pSource == '?')
                {
                    pSource++;
                    this.negative = true;
                }
                else
                    this.negative = false;
                this.Dimension = new(this, ref pSource, false);
            }
        }

        protected override DecoratedName GenerateCode()
        {
            DecoratedName result = new(this, this.Dimension.Code);
            if (this.negative) result.Prepend('?');
            return result;
        }
    }
}