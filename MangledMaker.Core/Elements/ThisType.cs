namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class ThisType : ComplexElement
    {
        public ThisType(ComplexElement parent)
            : base(parent)
        {
            this.DataIndirectType = new DataIndirectType(this,
                                                         new DecoratedName(), IndirectType.Null, new DecoratedName(), true);
        }

        public unsafe ThisType(ComplexElement parent, ref char* pSource)
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
            this.DataIndirectType = new DataIndirectType(this, ref pSource,
                                                         new DecoratedName(), IndirectType.Null, new DecoratedName(), true);
        }

        protected override DecoratedName GenerateCode()
        {
            return this.DataIndirectType.Code;
        }
    }
}