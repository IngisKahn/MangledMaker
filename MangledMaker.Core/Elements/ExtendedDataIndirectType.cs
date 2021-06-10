namespace MangledMaker.Core.Elements
{
    using Attributes;

    public enum IndirectType
    {
        Default,
        Pointer,
        Reference,
        Null = '\0'
    }

    public enum IndirectMode
    {
        Class,
        Pin,
        Fixed,
        ComPlus
    }


    public sealed class ExtendedDataIndirectType : Element
    {
        private int indirectionLevel;

        private bool isSubType;
        private bool isThis;

        public ExtendedDataIndirectType(Element parent) : base(parent)
        {
        }

        public ExtendedDataIndirectType(Element parent, IndirectMode mode, IndirectType indirection)
            : base(parent)
        {
            this.Mode = mode;
            this.Indirection = indirection;
        }

        public ExtendedDataIndirectType(Element parent, IndirectType indirection, int indirectionLevel, bool isSubType)
            : base(parent)
        {
            this.Mode = IndirectMode.ComPlus;
            this.Indirection = indirection;
            this.indirectionLevel = indirectionLevel;
            this.isSubType = isSubType;
        }

        public ExtendedDataIndirectType(Element parent, IndirectType indirection, bool isThis)
            : base(parent)
        {
            this.Mode = IndirectMode.Class;
            this.Indirection = indirection;
            this.isThis = isThis;
        }

        public unsafe ExtendedDataIndirectType(Element parent, ref char* pSource,
                                               IndirectType indirection, bool isThis)
            : this(parent, indirection, isThis) =>
            this.Parse(ref pSource);

        [Input]
        public IndirectType Indirection { get; set; }

        [Setting]
        public IndirectMode Mode { get; set; }

        [Output]
        public char Prefix
        {
            get
            {
                var prefix = this.Indirection switch
                {
                    IndirectType.Pointer => '*',
                    IndirectType.Reference => '&',
                    _ => '\0'
                };
                switch (this.Mode)
                {
                    case IndirectMode.Fixed:
                        prefix = '%';
                        break;
                    case IndirectMode.Class:
                        if (!this.isThis)
                            prefix = prefix == '&' ? '%' : '^';
                        break;
                }
                return prefix;
            }
        }

        [Input]
        public bool? IsThis
        {
            get => this.Mode == IndirectMode.Class ? this.isThis : null;
            set => this.isThis = value ?? false;
        }

        [Setting]
        public int? IndirectionLevel
        {
            get => this.indirectionLevel;
            set => this.indirectionLevel = value ?? 0;
        }

        [Setting]
        public bool? IsSubType
        {
            get => this.isSubType;
            set => this.isSubType = value ?? false;
        }

        protected override DecoratedName GenerateName()
        {
            if (this.Mode != IndirectMode.ComPlus)
                return new(this);

            DecoratedName szComPlusIndirSpecifier = new(this);

            if (this.indirectionLevel > 1)
                szComPlusIndirSpecifier.Assign(',' + new DecoratedName(this, (ulong)this.indirectionLevel));

            szComPlusIndirSpecifier += '>';
            if (this.isSubType)
                szComPlusIndirSpecifier += '^';

            szComPlusIndirSpecifier.IsComArray = true;
            return szComPlusIndirSpecifier;
        }

        private unsafe void Parse(ref char* pSource)
        {
            switch (*++pSource)
            {
                case 'A':
                    this.Mode = IndirectMode.Class;
                    pSource++;
                    return;
                case 'B':
                    this.Mode = IndirectMode.Pin;
                    pSource++;
                    return;
                case 'C':
                    this.Mode = IndirectMode.Fixed;
                    pSource++;
                    return;
            }

            this.Mode = IndirectMode.ComPlus;

            unchecked
            {
                this.indirectionLevel = (*pSource - '0' << 4) + (*(pSource + 1) - '0');
            }
            pSource += 2;

            this.isSubType = *pSource != '$';
            if (!this.isSubType)
                pSource++;

            pSource++;
        }

        protected override DecoratedName GenerateCode()
        {
            switch (this.Mode)
            {
                case IndirectMode.Class:
                    return new(this, 'A');
                case IndirectMode.Pin:
                    return new(this, 'B');
                case IndirectMode.Fixed:
                    return new(this, 'C');
                default:
                    var result = "";

                    unchecked
                    {
                        result += (char)(this.indirectionLevel >> 4 & 0xFFFF) + '0';
                        result += (char)(this.indirectionLevel & 0xFFFF) + '0';
                    }

                    if (!this.isSubType)
                        result += '$';
                    return new DecoratedName(this, result);
            }
        }
    }
}