namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class LexicalFrame : Element
    {
        public LexicalFrame(Element parent) : base(parent) => this.FrameIndex = new(this, 0);

        public unsafe LexicalFrame(Element parent, ref char* pSource) : base(parent) => this.FrameIndex = new(this, ref pSource, false);

        [Child]
        public Dimension FrameIndex { get; }

        protected override DecoratedName GenerateName() => '`' + new DecoratedName(this, this.FrameIndex.Name) + '\'';

        protected override DecoratedName GenerateCode() => this.FrameIndex.Code;
    }
}