namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class VdispMapType : ComplexElement
    {
        public VdispMapType(ComplexElement parent, DecoratedName superType)
            : base(parent)
        {
            this.SuperType = superType;
            this.Scope = new Scope(this);
        }

        public unsafe VdispMapType(ComplexElement parent, ref char* pSource,
                                   DecoratedName superType)
            : base(parent)
        {
            this.SuperType = superType;
            this.Parse(ref pSource);
        }

        [Input]
        public DecoratedName SuperType { get; set; }

        [Child]
        public Scope Scope { get; private set; }


        protected override DecoratedName GenerateName()
        {
            var vdispMapName = new DecoratedName(this, this.SuperType);
            vdispMapName += "{for ";
            vdispMapName += this.Scope.Name;
            vdispMapName += '}';
            return vdispMapName;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.Scope = new Scope(this, ref pSource);
            if (*pSource == '@')
                pSource++;
        }

        protected override DecoratedName GenerateCode()
        {
            var code = new DecoratedName(this);
            code += this.Scope.Code;
            code += '@';
            return code;
        }
    }
}