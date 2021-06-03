namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class GuardNumber : Element
    {
        public GuardNumber(Element parent) : base(parent)
        {
            this.Number = new Dimension(this, 0);
        }

        public unsafe GuardNumber(Element parent, ref char* pSource) : base(parent)
        {
            this.Parse(ref pSource);
        }

        [Child]
        public Dimension Number { get; private set; }

        protected override DecoratedName GenerateName()
        {
            return this.Number.Name;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.Number = new Dimension(this, ref pSource, false);
        }

        protected override DecoratedName GenerateCode()
        {
            return this.Number.Code;
        }
    }
}