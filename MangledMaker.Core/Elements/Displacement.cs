namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class Displacement : Element
    {
        [Child]
        public Dimension Offset { get; private set; }

        public Displacement(Element parent) : base(parent) { this.Offset = new Dimension(this, 0, true); }
        public unsafe Displacement(Element parent, ref char* pSource) : base(parent) { Parse(ref pSource); }

        protected override DecoratedName GenerateName()
        {
            return this.Offset.Name;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.Offset = new Dimension(this, ref pSource, true);
        }

        protected override DecoratedName GenerateCode()
        {
            return this.Offset.Code;
        }
    }
}