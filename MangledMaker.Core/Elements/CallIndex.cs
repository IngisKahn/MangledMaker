namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class CallIndex : Element
    {
        public CallIndex(Element parent) : base(parent) => this.Index = new(this, 0);

        public unsafe CallIndex(Element parent, ref char* pSource) : base(parent) => this.Index = new(this, ref pSource, false);

        [Child]
        public Dimension Index { get; }

        protected override DecoratedName GenerateName() => this.Index.Name;

        protected override DecoratedName GenerateCode() => this.Index.Code;
    }
}