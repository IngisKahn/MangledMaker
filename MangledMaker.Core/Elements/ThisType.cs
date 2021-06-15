namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class ThisType : ComplexElement
    {
        public ThisType(ComplexElement parent)
            : base(parent) =>
            this.DataIndirectType = new(this,
                new(), IndirectType.Null, new(), true);

        public unsafe ThisType(ComplexElement parent, ref char* pSource)
            : base(parent) =>
            this.DataIndirectType = new(this, ref pSource,
                new(), IndirectType.Null, new(), true);

        [Child]
        public DataIndirectType DataIndirectType { get; private set; }


        protected override DecoratedName GenerateName() => this.DataIndirectType.Name;

        protected override DecoratedName GenerateCode() => this.DataIndirectType.Code;
    }
}