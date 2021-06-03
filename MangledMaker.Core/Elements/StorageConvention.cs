namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class StorageConvention : ComplexElement
    {
        public StorageConvention(ComplexElement parent)
            : base(parent)
        {
            this.DataIndirectType = new DataIndirectType(this);
        }

        public unsafe StorageConvention(ComplexElement parent, ref char* pSource)
            : base(parent)
        {
            this.Parse(ref pSource);
        }

        [Child]
        public DataIndirectType DataIndirectType { get; private set; }


        protected override DecoratedName GenerateName()
        {
            return this.DataIndirectType.Name;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.DataIndirectType = new DataIndirectType(this, ref pSource);
        }

        protected override DecoratedName GenerateCode()
        {
            return this.DataIndirectType.Code;
        }
    }
}