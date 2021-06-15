namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class PointerTypeArray : ComplexElement
    {
        public PointerTypeArray(ComplexElement parent, DecoratedName superType,
                                DecoratedName cvType)
            : base(parent)
        {
            this.SuperType = superType;
            this.CvType = cvType;
        }

        public unsafe PointerTypeArray(ComplexElement parent, ref char* pSource,
                                       DecoratedName superType, DecoratedName cvType)
            : this(parent, superType, cvType) =>
            this.ptrRefType = new(this, ref pSource, this.CvType, this.SuperType, IndirectType.Null);

        [Input]
        public DecoratedName SuperType { get; set; }

        [Input]
        public DecoratedName CvType { get; set; }

        private PtrRefType? ptrRefType;

        [Child] public PtrRefType PtrRefType => this.ptrRefType ??= new(this, this.CvType, this.SuperType, IndirectType.Null);

        protected override DecoratedName GenerateName()
        {
            this.PtrRefType.CvType = this.CvType;
            this.PtrRefType.SuperType = this.SuperType;
            return this.PtrRefType.Name;
        }

        protected override DecoratedName GenerateCode()
        {
            this.PtrRefType.CvType = this.CvType;
            this.PtrRefType.SuperType = this.SuperType;
            return this.PtrRefType.Code;
        }
    }
}