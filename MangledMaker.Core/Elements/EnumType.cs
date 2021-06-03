namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class EnumType : Element
    {
        public enum EnumBaseType
        {
            Char,
            Short,
            Int,
            Long
        }

        public EnumType(Element parent, EnumBaseType baseType = EnumBaseType.Int, bool signed = true) : base(parent)
        {
            this.BaseType = baseType;
            this.Signed = signed;
        }

        public unsafe EnumType(Element parent, ref char* pSource) : base(parent)
        {
            this.Parse(ref pSource);
        }

        [Setting]
        public EnumBaseType BaseType { get; set; }

        [Setting]
        public bool Signed { get; set; }

        protected override DecoratedName GenerateName()
        {
            var ecsuName = new DecoratedName(this);

            switch (this.BaseType)
            {
                case EnumBaseType.Char:
                    ecsuName.Assign("char ");
                    break;
                case EnumBaseType.Short:
                    ecsuName.Assign("short ");
                    break;
                case EnumBaseType.Int:
                    if (!this.Signed) ecsuName.Assign("int ");
                    break;
                case EnumBaseType.Long:
                    ecsuName.Assign("long ");
                    break;
            }

            if (!this.Signed)
                ecsuName.Assign("unsigned " + ecsuName);

            return ecsuName;
        }

        private unsafe void Parse(ref char* pSource)
        {
            if (*pSource == '\0')
            {
                this.IsTruncated = true;
                return;
            }

            switch (*pSource)
            {
                case '0':
                case '1':
                    this.BaseType = EnumBaseType.Char;
                    break;
                case '2':
                case '3':
                    this.BaseType = EnumBaseType.Short;
                    break;
                case '4':
                case '5':
                    this.BaseType = EnumBaseType.Int;
                    break;
                case '6':
                case '7':
                    this.BaseType = EnumBaseType.Long;
                    break;
                default:
                    this.IsInvalid = true;
                    break;
            }
            this.Signed = true;
            switch (*pSource++)
            {
                case '1':
                case '3':
                case '5':
                case '7':
                    this.Signed = false;
                    break;
            }
        }

        protected override DecoratedName GenerateCode()
        {
            var value = (int)this.BaseType;
            value <<= 1;
            if (!this.Signed)
                value++;
            return new DecoratedName(this, (char)(value + '0'));
        }
    }
}