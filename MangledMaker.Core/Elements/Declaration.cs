namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class Declaration : ComplexElement
    {
        private Call call;
        private SpecialName specialName;

        public Declaration(ComplexElement parent, DecoratedName symbol)
            : base(parent)
        {
            this.Symbol = symbol;
        }

        public unsafe Declaration(ComplexElement parent, ref char* pSource,
                                  DecoratedName symbol)
            : base(parent)
        {
            this.Symbol = symbol;
            this.Parse(ref pSource);
        }

        [Input]
        public DecoratedName Symbol { get; set; }

        [Child]
        public TypeEncoding TypeEncoding { get; private set; }

        [Child]
        public Call Call
        {
            get { return this.IsCall ? this.call : null; }
        }

        [Child]
        public SpecialName SpecialName
        {
            get { return !this.IsCall ? this.specialName : null; }
        }

        private bool IsCall
        {
            get
            {
                return this.TypeEncoding != null &&
                       (this.TypeEncoding.IsFunction &&
                        !(this.TypeEncoding.IsDestructorHelper ||
                          this.TypeEncoding.IsDataMemberConstructorHelper ||
                          this.TypeEncoding.IsDataMemberDestructorHelper));
            }
        }

        protected override void CreateEmptyElements()
        {
            if (this.TypeEncoding == null)
                this.TypeEncoding = new TypeEncoding(this);
            if (this.call == null)
                this.call = new Call(this, new DecoratedName(), new DecoratedName(), this.TypeEncoding);
            if (this.specialName == null)
                this.specialName = new SpecialName(this, this.TypeEncoding, new DecoratedName());
        }

        protected override DecoratedName GenerateName()
        {
            var declaration = new DecoratedName(this);

            if (!this.TypeEncoding.Name.IsValid)
                return new DecoratedName(this, NodeStatus.Invalid);

            if (this.TypeEncoding.Name.IsMissing)
                return new DecoratedName(this, NodeStatus.Truncated) + this.Symbol;

            if (this.TypeEncoding.IsNone)
                return new DecoratedName(this, this.Symbol);

            bool needsModifiers;

            if (this.IsCall)
            {
                this.call.Declaration = declaration;
                this.call.Symbol = this.Symbol;
                declaration = this.call.Name;
                needsModifiers = this.call.NeedsModifiers;
            }
            else
            {
                this.specialName.Symbol = declaration.Append(this.Symbol);
                declaration = this.specialName.Name;
                needsModifiers = this.specialName.NeedsModifiers;
            }

            // reset parent
            declaration = new DecoratedName(this, declaration);

            if (!needsModifiers) 
                return declaration;
            if (this.UnDecorator.DoMemberTypes)
            {
                if (this.TypeEncoding.IsMemberStatic)
                    declaration.Prepend("static ");
                if (this.TypeEncoding.IsMemberVirtual)
                    declaration.Prepend("virtual ");
            }
            if (this.UnDecorator.DoAccessSpecifiers)
                if (this.TypeEncoding.IsPrivate)
                    declaration.Prepend("private: ");
                else if (this.TypeEncoding.IsProtected)
                    declaration.Prepend("protected: ");
                else if (this.TypeEncoding.IsPublic)
                    declaration.Prepend("public: ");
            if (this.TypeEncoding.IsThunk && !this.UnDecorator.DoNameOnly)
                declaration.Prepend("[thunk]: ");
            if (this.TypeEncoding.IsExternC == true)
                declaration.Prepend("extern \"C\" ");

            return declaration;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.TypeEncoding = new TypeEncoding(this, ref pSource);

            if (!this.TypeEncoding.Name.IsValid || this.TypeEncoding.Name.IsMissing || this.TypeEncoding.IsNone)
                return;

            if (this.IsCall)
                this.call = new Call(this, ref pSource, new DecoratedName(), this.Symbol,
                                     this.TypeEncoding);
            else
                this.specialName = new SpecialName(this, ref pSource, this.TypeEncoding, this.Symbol);
        }

        protected override DecoratedName GenerateCode()
        {
            var code = new DecoratedName(this, this.TypeEncoding.Code);
            code.Append(this.IsCall ? this.call.Code : this.specialName.Code);

            return code;
        }
    }
}