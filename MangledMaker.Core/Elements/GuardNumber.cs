namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class GuardNumber : Element
    {
        public GuardNumber(Element parent) : base(parent) => this.Number = new(this, 0);

        public unsafe GuardNumber(Element parent, ref char* pSource) : base(parent) => this.Number = new(this, ref pSource, false);

        [Child]
        public Dimension Number { get; }

        protected override DecoratedName GenerateName() => this.Number.Name;

        protected override DecoratedName GenerateCode() => this.Number.Code;
    }
}