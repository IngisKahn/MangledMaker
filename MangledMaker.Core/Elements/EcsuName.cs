namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class EcsuName : ComplexElement
    {
        public EcsuName(ComplexElement parent)
            : base(parent) =>
            this.ScopedName = new ScopedName(this);

        public unsafe EcsuName(ComplexElement parent, ref char* pSource)
            : base(parent) =>
            this.ScopedName = new ScopedName(this, ref pSource);

        [Child]
        public ScopedName ScopedName { get; }


        protected override DecoratedName GenerateName() => this.ScopedName.Name;

        protected override DecoratedName GenerateCode() => this.ScopedName.Code;
    }
}