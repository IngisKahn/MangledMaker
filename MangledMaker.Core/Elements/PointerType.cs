namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class PointerType : ComplexElement
    {
        public PointerType(ComplexElement parent, DecoratedName superType,
                           DecoratedName cvType)
            : base(parent)
        {
            this.SuperType = superType;
            this.CvType = cvType;
        }

        public unsafe PointerType(ComplexElement parent, ref char* pSource,
                                  DecoratedName superType, DecoratedName cvType)
            : this(parent, superType, cvType) =>
            this.PtrRefType = new(this, ref pSource, this.CvType, this.SuperType,
                IndirectType.Pointer);

        [Input]
        public DecoratedName SuperType { get; set; }

        [Input]
        public DecoratedName CvType { get; set; }

        private PtrRefType? ptrRefType;

        [Child]
        public PtrRefType PtrRefType
        {
            get => this.ptrRefType ??= new(this, this.CvType, this.SuperType, IndirectType.Pointer);
            private init => this.ptrRefType = value;
        }

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