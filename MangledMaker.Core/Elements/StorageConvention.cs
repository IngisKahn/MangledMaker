namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class StorageConvention : ComplexElement
    {
        public StorageConvention(ComplexElement parent)
            : base(parent) =>
            this.DataIndirectType = new(this);

        public unsafe StorageConvention(ComplexElement parent, ref char* pSource)
            : base(parent) =>
            this.DataIndirectType = new(this, ref pSource);

        [Child]
        public DataIndirectType DataIndirectType { get; private set; }


        protected override DecoratedName GenerateName() => this.DataIndirectType.Name;

        protected override DecoratedName GenerateCode() => this.DataIndirectType.Code;
    }
}