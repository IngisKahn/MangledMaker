namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class VdispMapType : ComplexElement
    {
        public VdispMapType(ComplexElement parent, DecoratedName superType)
            : base(parent)
        {
            this.SuperType = superType;
            this.Scope = new(this);
        }

        public unsafe VdispMapType(ComplexElement parent, ref char* pSource,
                                   DecoratedName superType)
            : base(parent)
        {
            this.SuperType = superType;
            this.Scope = new(this, ref pSource);
            if (*pSource == '@')
                pSource++;
        }

        [Input]
        public DecoratedName SuperType { get; set; }

        [Child]
        public Scope Scope { get; private set; }


        protected override DecoratedName GenerateName()
        {
            DecoratedName vdispMapName = new(this, this.SuperType);
            vdispMapName += "{for ";
            vdispMapName += this.Scope.Name;
            vdispMapName += '}';
            return vdispMapName;
        }

        protected override DecoratedName GenerateCode()
        {
            DecoratedName code = new(this);
            code += this.Scope.Code;
            code += '@';
            return code;
        }
    }
}