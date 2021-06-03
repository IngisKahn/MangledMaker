namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class ExternalDataType : ComplexElement
    {
        public ExternalDataType(ComplexElement parent, DecoratedName superType)
            : base(parent)
        {
            this.SuperType = superType;
            this.DataType = new DataType(this, new DecoratedName());
            this.StorageConvention = new StorageConvention(this);
        }

        public unsafe ExternalDataType(ComplexElement parent, ref char* pSource,
                                       DecoratedName superType)
            : base(parent)
        {
            this.SuperType = superType;
            this.Parse(ref pSource);
        }

        [Input]
        public DecoratedName SuperType { get; set; }

        [Child]
        public DataType DataType { get; private set; }

        [Child]
        public StorageConvention StorageConvention { get; private set; }


        protected override DecoratedName GenerateName()
        {
            var pDeclarator = new DecoratedName(this);
            this.DataType.Declarator = pDeclarator;
            var declaration = this.DataType.Name;
            pDeclarator.Assign(this.StorageConvention.Name);
            pDeclarator.Append(' ');
            pDeclarator.Append(this.SuperType);
            return declaration;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.DataType = new DataType(this, ref pSource, new DecoratedName());
            this.StorageConvention = new StorageConvention(this, ref pSource);
        }

        protected override DecoratedName GenerateCode()
        {
            return this.DataType.Code + this.StorageConvention.Code;
        }
    }
}