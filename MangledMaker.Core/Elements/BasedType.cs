namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class BasedType : ComplexElement
    {
        public enum BasedTypeMode
        {
            Empty,
            Void,
            Named
        }

        public BasedType(ComplexElement parent)
            : base(parent)
        {
            this.BasedName = new ScopedName(this);
        }

        public unsafe BasedType(ComplexElement parent, ref char* pSource)
            : base(parent)
        {
            this.Parse(ref pSource);
        }

        [Setting]
        public BasedTypeMode Mode { get; set; }

        [Child]
        public ScopedName BasedName { get; private set; }


        protected override DecoratedName GenerateName()
        {
            var result = new DecoratedName(this, this.UScore(TokenType.Based));
            switch (this.Mode)
            {
                case BasedTypeMode.Void:
                    result.Append("void");
                    break;
                case BasedTypeMode.Named:
                    result.Append(this.BasedName.Name);
                    break;
            }
            return result + ") ";
        }

        private unsafe void Parse(ref char* pSource)
        {
            if (*pSource == '\0')
            {
                this.IsTruncated = true;
                return;
            }

            switch (*pSource++)
            {
                case '0':
                    this.Mode = BasedTypeMode.Void;
                    break;
                case '2':
                    this.Mode = BasedTypeMode.Named;
                    this.BasedName = new ScopedName(this, ref pSource);
                    break;
                case '5':
                    this.IsInvalid = true;
                    break;
                default:
                    this.Mode = BasedTypeMode.Empty;
                    break;
            }
        }

        protected override DecoratedName GenerateCode()
        {
            switch (this.Mode)
            {
                case BasedTypeMode.Void:
                    return new DecoratedName(this, '0');
                case BasedTypeMode.Named:
                    return new DecoratedName(this, '2') + this.BasedName.Code;
                default:
                    return new DecoratedName(this, '1');
            }
        }
    }
}