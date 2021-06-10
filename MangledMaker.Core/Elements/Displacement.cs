namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class Displacement : Element
    {
        [Child]
        public Dimension Offset { get; }

        public Displacement(Element parent) : base(parent) => this.Offset = new(this, 0, true);
        public unsafe Displacement(Element parent, ref char* pSource) : base(parent) => this.Offset = new(this, ref pSource, true);

        protected override DecoratedName GenerateName() => this.Offset.Name;

        protected override DecoratedName GenerateCode() => this.Offset.Code;
    }
}