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
            this.DataType = new(this, ref pSource, new());
            this.StorageConvention = new(this, ref pSource);
        }

        [Input]
        public DecoratedName SuperType { get; set; }

        [Child]
        public DataType DataType { get; }

        [Child]
        public StorageConvention StorageConvention { get; }


        protected override DecoratedName GenerateName()
        {
            DecoratedName pDeclarator = new(this);
            this.DataType.Declarator = pDeclarator;
            var declaration = this.DataType.Name;
            pDeclarator.Assign(this.StorageConvention.Name);
            pDeclarator.Append(' ');
            pDeclarator.Append(this.SuperType);
            return declaration;
        }

        protected override DecoratedName GenerateCode() => this.DataType.Code + this.StorageConvention.Code;
    }
}