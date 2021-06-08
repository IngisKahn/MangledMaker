namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class UserDefinedType : ComplexElement
    {
        public enum Ecsus
        {
            //None,
            Union,
            Struct,
            Class,
            Enum,
            Coclass,
            Cointerface
        }

        private Ecsus ecsu;

        private EnumType enumType;

        private bool isMissing;

        public UserDefinedType(ComplexElement parent)
            : base(parent)
        { }

        public unsafe UserDefinedType(ComplexElement parent, ref char* pSource)
            : this(parent)
        {
            this.Parse(ref pSource);
        }

        [Setting]
        public Ecsus Ecsu
        {
            get { return this.ecsu; }
            set
            {
                this.ecsu = value;
                this.isMissing = false;
            }
        }

        [Child]
        public EnumType EnumType
        {
            get { return this.ecsu == Ecsus.Enum ? this.enumType : null; }
        }

        [Child]
        public EcsuName EcsuName { get; private set; }

        protected override void CreateEmptyElements()
        {
            if (this.enumType == null) this.enumType = new EnumType(this);
            if (this.EcsuName == null) this.EcsuName = new EcsuName(this);
        }

        protected override DecoratedName GenerateName()
        {
            if (this.isMissing)
                return new DecoratedName(this, "`unknown ecsu'");

            var doPrefix = this.UnDecorator.DoEcsu && !this.UnDecorator.DoNameOnly;
            var prefix = new DecoratedName(this);

            switch (this.ecsu)
            {
                case Ecsus.Union:
                    prefix.Assign("union ");
                    break;
                case Ecsus.Struct:
                    prefix.Assign("struct ");
                    break;
                case Ecsus.Class:
                    prefix.Assign("class ");
                    break;
                case Ecsus.Enum:
                    doPrefix = this.UnDecorator.DoEcsu;
                    prefix.Assign("enum ");
                    prefix += this.enumType.Name;
                    break;
                case Ecsus.Coclass:
                    prefix.Assign("coclass ");
                    break;
                case Ecsus.Cointerface:
                    prefix.Assign("cointerface ");
                    break;
            }
            var ecsuDataType = new DecoratedName(this);
            if (doPrefix)
                ecsuDataType.Assign(prefix);

            return new DecoratedName(ecsuDataType.Append(this.EcsuName.Name));
        }

        private unsafe void Parse(ref char* pSource)
        {
            switch (*pSource++)
            {
                case '\0':
                    pSource--;
                    this.isMissing = true;
                    return;
                case 'T':
                    this.ecsu = Ecsus.Union;
                    break;
                case 'U':
                    this.ecsu = Ecsus.Struct;
                    break;
                case 'V':
                    this.ecsu = Ecsus.Class;
                    break;
                case 'W':
                    this.ecsu = Ecsus.Enum;
                    this.enumType = new EnumType(this, ref pSource);
                    break;
                case 'X':
                    this.ecsu = Ecsus.Coclass;
                    break;
                case 'Y':
                    this.ecsu = Ecsus.Cointerface;
                    break;
            }

            this.EcsuName = new EcsuName(this, ref pSource);
        }

        protected override DecoratedName GenerateCode()
        {
            var code = new DecoratedName(this);

            if (this.isMissing) 
                return code + this.EcsuName.Code;
            switch (this.ecsu)
            {
                case Ecsus.Union:
                    code.Assign('T');
                    break;
                case Ecsus.Struct:
                    code.Assign('U');
                    break;
                case Ecsus.Class:
                    code.Assign('V');
                    break;
                case Ecsus.Enum:
                    code.Assign('W');
                    code += this.enumType.Code;
                    break;
                case Ecsus.Coclass:
                    code.Assign('X');
                    break;
                case Ecsus.Cointerface:
                    code.Assign('Y');
                    break;
            }
            return code + this.EcsuName.Code;
        }
    }
}