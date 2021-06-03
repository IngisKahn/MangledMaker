namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class CallIndex : Element
    {
        public CallIndex(Element parent) : base(parent)
        {
            this.Index = new Dimension(this, 0);
        }

        public unsafe CallIndex(Element parent, ref char* pSource) : base(parent)
        {
            this.Parse(ref pSource);
        }

        [Child]
        public Dimension Index { get; private set; }

        protected override DecoratedName GenerateName()
        {
            return this.Index.Name;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.Index = new Dimension(this, ref pSource, false);
        }

        protected override DecoratedName GenerateCode()
        {
            return this.Index.Code;
        }
    }
}