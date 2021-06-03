namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class LexicalFrame : Element
    {
        public LexicalFrame(Element parent) : base(parent)
        {
            this.FrameIndex = new Dimension(this, 0);
        }

        public unsafe LexicalFrame(Element parent, ref char* pSource) : base(parent)
        {
            this.Parse(ref pSource);
        }

        [Child]
        public Dimension FrameIndex { get; private set; }

        protected override DecoratedName GenerateName()
        {
            return '`' + new DecoratedName(this, this.FrameIndex.Name) + '\'';
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.FrameIndex = new Dimension(this, ref pSource, false);
        }

        protected override DecoratedName GenerateCode()
        {
            return this.FrameIndex.Code;
        }
    }
}