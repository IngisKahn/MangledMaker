namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class ReferenceType : ComplexElement
    {
        public ReferenceType(ComplexElement parent, DecoratedName superType,
                             DecoratedName cvType)
            : base(parent)
        {
            this.SuperType = superType;
            this.CvType = cvType;
        }

        public unsafe ReferenceType(ComplexElement parent, ref char* pSource,
                                    DecoratedName superType, DecoratedName cvType)
            : this(parent, superType, cvType) =>
            this.ptrRefType = new(this, ref pSource, this.CvType, this.SuperType,
                IndirectType.Reference);

        [Input]
        public DecoratedName SuperType { get; set; }

        [Input]
        public DecoratedName CvType { get; set; }

        private PtrRefType? ptrRefType;
        [Child]
        public PtrRefType PtrRefType => this.ptrRefType ??= new(this, this.CvType, this.SuperType,
            IndirectType.Reference);

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