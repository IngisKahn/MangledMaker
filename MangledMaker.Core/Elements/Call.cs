namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class Call : ComplexElement
    {
        private BasedType? basedType;
        private Function? function;
        private TypedThunk? typedThunk;

        public Call(ComplexElement parent, DecoratedName declaration, DecoratedName symbol,
                    TypeEncoding typeEncoding)
            : base(parent)
        {
            this.Symbol = symbol;
            this.TypeEncoding = typeEncoding;
            this.Declaration = declaration;
        }

        public unsafe Call(ComplexElement parent, ref char* pSource,
                           DecoratedName declaration, DecoratedName symbol, TypeEncoding typeEncoding)
            : base(parent)
        {
            this.Symbol = symbol;
            this.TypeEncoding = typeEncoding;
            this.Declaration = declaration;
            this.Parse(ref pSource);
        }

        [Input]
        public DecoratedName Symbol { get; set; }

        [Input]
        public DecoratedName Declaration { get; set; }

        [Input]
        public TypeEncoding TypeEncoding { get; set; }

        [Child]
        public BasedType? BasedType => this.TypeEncoding.IsBased ?? false ? this.basedType ??= new(this) : null;

        [Child]
        public TypedThunk? TypedThunk => this.TypeEncoding.IsTypedThunk ? this.typedThunk ??= new(this, this.Declaration, this.Symbol, this.TypeEncoding) : null;

        [Child]
        public Function? Function => !this.TypeEncoding.IsTypedThunk ? this.function ??= new(this, this.Declaration, this.Symbol, this.TypeEncoding) : null;

        [Output]
        public bool NeedsModifiers { get; private set; }

        protected override DecoratedName GenerateName()
        {
            var declaration = new DecoratedName(this, this.Declaration);

            if ((this.TypeEncoding.IsBased ?? false) && this.basedType != null)
                if (this.UnDecorator.DoMicrosoftKeywords && this.UnDecorator.DoAllocationModel)
                {
                    declaration.Append(' ');
                    declaration.Append(this.basedType.Name);
                }
                else
                    declaration.Skip(this.basedType.Name);


            if (this.TypeEncoding.IsTypedThunk && this.typedThunk != null)
            {
                this.typedThunk.Declaration = declaration;
                this.typedThunk.Symbol = this.Symbol;
                declaration = this.typedThunk.Name;

                this.NeedsModifiers = true;
            }
            else if (this.function != null)
            {
                this.function.Declaration = declaration;
                this.function.Symbol = this.Symbol;
                declaration = this.function.Name;
                this.NeedsModifiers = this.function.NeedsModifiers;
            }

            return declaration;
        }

        private unsafe void Parse(ref char* pSource)
        {
            if (this.TypeEncoding.IsBased ?? false)
                this.basedType = new(this);

            if (this.TypeEncoding.IsTypedThunk)
                this.typedThunk = new(this, ref pSource, this.Declaration, this.Symbol,
                                                 this.TypeEncoding);
            else
                this.function = new(this, ref pSource, this.Declaration, this.Symbol,
                                             this.TypeEncoding);
        }

        protected override DecoratedName GenerateCode()
        {
            DecoratedName code = new(this);

            if ((this.TypeEncoding.IsBased ?? false) && this.basedType != null)
                code.Assign(this.basedType.Code);

            if (this.TypeEncoding.IsTypedThunk && this.typedThunk != null)
                return code + this.typedThunk.Code;
            return this.function != null ? code + this.function.Code : code;
        }
    }
}