namespace MangledMaker.Core.Elements
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class VCallThunkType : Element
    {
        private DecoratedName name;

        public VCallThunkType(Element parent) : base(parent) => this.MakeFlat();

        public unsafe VCallThunkType(Element parent, ref char* pSource) : base(parent) => this.Parse(ref pSource);

        [MemberNotNull(nameof(VCallThunkType.name))]
        private void MakeFlat() => this.name = new(this, "{flat}");

        protected override DecoratedName GenerateName() => this.name;

        [MemberNotNull(nameof(VCallThunkType.name))]
        private unsafe void Parse(ref char* pSource)
        {
            switch (*pSource)
            {
                case '\0':
                    this.name = new(NodeStatus.Truncated);
                    break;
                case 'A':
                    pSource++;
                    this.MakeFlat();
                    break;
                default:
                    this.name = new(NodeStatus.Invalid);
                    break;
            }
        }

        protected override DecoratedName GenerateCode() => new(this, 'A');
    }
}