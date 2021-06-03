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
            this.Dimension = new Dimension(this, 0);
        }

        public unsafe SignedDimension(Element parent, ref char* pSource) : base(parent)
        {
            this.Parse(ref pSource);
        }

        public bool FixedSign { get; set; }

        [Setting]
        public bool? Negative
        {
            get { return this.FixedSign ? (bool?)null : this.negative; }
            set { if (value != null) this.negative = (bool)value; }
        }

        [Child]
        public Dimension Dimension { get; private set; }

        protected override DecoratedName GenerateName()
        {
            if (this.negative)
                return '-' + new DecoratedName(this, this.Dimension.Name);
            return this.Dimension.Name;
        }

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
                this.Dimension = new Dimension(this, ref pSource, false);
            }
        }

        protected override DecoratedName GenerateCode()
        {
            var result = new DecoratedName(this, this.Dimension.Code);
            if (this.negative) result.Prepend('?');
            return result;
        }
    }
}