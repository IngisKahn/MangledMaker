namespace MangledMaker.Core.Elements
{
    public sealed class VCallThunkType : Element
    {
        private DecoratedName name;

        public VCallThunkType(Element parent) : base(parent)
        {
            this.MakeFlat();
        }

        public unsafe VCallThunkType(Element parent, ref char* pSource) : base(parent)
        {
            this.Parse(ref pSource);
        }

        private void MakeFlat()
        {
            this.name = new DecoratedName(this, "{flat}");
        }

        protected override DecoratedName GenerateName()
        {
            return this.name;
        }

        private unsafe void Parse(ref char* pSource)
        {
            switch (*pSource)
            {
                case '\0':
                    this.name = new DecoratedName(NodeStatus.Truncated);
                    break;
                case 'A':
                    pSource++;
                    this.MakeFlat();
                    break;
                default:
                    this.name = new DecoratedName(NodeStatus.Invalid);
                    break;
            }
        }

        protected override DecoratedName GenerateCode()
        {
            return new DecoratedName(this, 'A');
        }
    }
}