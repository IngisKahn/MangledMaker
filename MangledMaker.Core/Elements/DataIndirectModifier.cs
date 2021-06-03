namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class DataSpecialModifier : ComplexElement
    {
        public enum StandardIndirectModifier
        {
            Pointer64,
            Restrict,
            Unaligned
        }

        private ExtendedDataIndirectType extendedDataIndirectType;

        public DataSpecialModifier(ComplexElement parent)
            : base(parent)
        {
        }

        public DataSpecialModifier(ComplexElement parent,
                                   IndirectType indirection, bool isThis,
                                   DecoratedName keywords, DecoratedName unaligned)
            : base(parent)
        {
            this.Keywords = keywords;
            this.Unaligned = unaligned;
            this.Indirection = indirection;
            this.IsThis = isThis;
        }

        public unsafe DataSpecialModifier(ComplexElement parent, ref char* pSource,
                                          IndirectType indirection, bool isThis, DecoratedName keywords,
                                          DecoratedName unaligned)
            : this(parent, indirection, isThis, keywords, unaligned)
        {
            this.Parse(ref pSource);
        }

        [Input]
        public DecoratedName Keywords { get; set; }

        [Input]
        public DecoratedName Unaligned { get; set; }

        [Input]
        public IndirectType Indirection { get; set; }

        [Setting]
        public StandardIndirectModifier Modifier { get; set; }

        [Child]
        public ExtendedDataIndirectType ExtendedDataIndirectType
        {
            get { return this.IsExtended ? this.extendedDataIndirectType : null; }
        }

        [Output]
        public char Prefix
        {
            get
            {
                if (this.IsExtended)
                    return this.extendedDataIndirectType.Prefix;

                var prefix = '\0';
                switch (this.Indirection)
                {
                    case IndirectType.Pointer:
                        prefix = '*';
                        break;
                    case IndirectType.Reference:
                        prefix = '&';
                        break;
                }
                return prefix;
            }
        }

        [Input]
        public bool IsThis { get; set; }

        [Setting]
        public bool IsExtended { get; set; }

        protected override void CreateEmptyElements()
        {
            if (this.extendedDataIndirectType == null)
                this.extendedDataIndirectType = new ExtendedDataIndirectType(this, this.Indirection, this.IsThis);
        }

        protected override DecoratedName GenerateName()
        {
            if (this.IsExtended)
            {
                this.extendedDataIndirectType.Indirection = this.Indirection;
                this.extendedDataIndirectType.IsThis = this.IsThis;
                var name = this.extendedDataIndirectType.Name;
                if (!name.IsEmpty)
                    return name;
            }

            if (!this.UnDecorator.DoMicrosoftKeywords) 
                return new DecoratedName(this);
            switch (this.Modifier)
            {
                case StandardIndirectModifier.Pointer64:
                    if (this.UnDecorator.DoPtr64)
                    {
                        var ptr = new DecoratedName(this, this.UScore(TokenType.Pointer));
                        if (!this.Keywords.IsEmpty)
                            ptr.Prepend(' ');
                        this.Keywords.Append(ptr);
                    }
                    break;
                case StandardIndirectModifier.Restrict:
                    var res = new DecoratedName(this, this.UScore(TokenType.Restrict));
                    if (!this.Keywords.IsEmpty)
                        res.Prepend(' ');
                    this.Keywords.Append(res);
                    break;
                case StandardIndirectModifier.Unaligned:
                    var unl = new DecoratedName(this, this.UScore(TokenType.Unaligned));
                    if (!this.Unaligned.IsEmpty)
                        unl.Prepend(' ');
                    this.Unaligned.Append(unl);
                    break;
            }

            return new DecoratedName(this);
        }

        private unsafe void Parse(ref char* pSource)
        {
            if (*pSource == '$')
            {
                this.IsExtended = true;
                this.extendedDataIndirectType = new ExtendedDataIndirectType(this, ref pSource, this.Indirection,
                                                                             this.IsThis);
                if (!this.extendedDataIndirectType.Name.IsEmpty)
                    return;
            }
            else
                this.IsExtended = false;

            switch (*pSource++)
            {
                case 'E':
                    this.Modifier = StandardIndirectModifier.Pointer64;
                    break;
                case 'F':
                    this.Modifier = StandardIndirectModifier.Unaligned;
                    break;
                case 'I':
                    this.Modifier = StandardIndirectModifier.Restrict;
                    break;
                default:
                    pSource--;
                    break;
            }
        }

        protected override DecoratedName GenerateCode()
        {
            var code = new DecoratedName(this);
            if (this.IsExtended)
            {
                code.Append('$');
                code.Append(this.extendedDataIndirectType.Code);
                if (!this.extendedDataIndirectType.Name.IsEmpty)
                    return code;
            }

            switch (this.Modifier)
            {
                case StandardIndirectModifier.Pointer64:
                    code.Append('E');
                    break;
                case StandardIndirectModifier.Unaligned:
                    code.Append('F');
                    break;
                case StandardIndirectModifier.Restrict:
                    code.Append('I');
                    break;
            }

            return code;
        }
    }
}