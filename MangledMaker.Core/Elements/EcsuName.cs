namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class EcsuName : ComplexElement
    {
        public EcsuName(ComplexElement parent)
            : base(parent)
        {
            this.ScopedName = new ScopedName(this);
        }

        public unsafe EcsuName(ComplexElement parent, ref char* pSource)
            : base(parent)
        {
            this.Parse(ref pSource);
        }

        [Child]
        public ScopedName ScopedName { get; private set; }


        protected override DecoratedName GenerateName()
        {
            return this.ScopedName.Name;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.ScopedName = new ScopedName(this, ref pSource);
        }

        protected override DecoratedName GenerateCode()
        {
            return this.ScopedName.Code;
        }
    }
}