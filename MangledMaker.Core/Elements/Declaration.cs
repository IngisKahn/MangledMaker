namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class Declaration : ComplexElement
    {
        private Call? call;
        private SpecialName? specialName;

        public Declaration(ComplexElement parent, DecoratedName symbol)
            : base(parent) =>
            this.Symbol = symbol;

        public unsafe Declaration(ComplexElement parent, ref char* pSource,
                                  DecoratedName symbol)
            : base(parent)
        {
            this.Symbol = symbol;
            this.Parse(ref pSource);
        }

        [Input]
        public DecoratedName Symbol { get; set; }

        private TypeEncoding? typeEncoding;

        [Child] public TypeEncoding TypeEncoding => this.typeEncoding ??= new(this);

        [Child]
        public Call? Call => this.IsCall ? this.call : null;

        [Child]
        public SpecialName? SpecialName => !this.IsCall ? this.specialName : null;

        private bool IsCall =>
            this.TypeEncoding is {IsFunction: true} && !(this.TypeEncoding.IsDestructorHelper ||
                                                         this.TypeEncoding.IsDataMemberConstructorHelper ||
                                                         this.TypeEncoding.IsDataMemberDestructorHelper);

        private Call CallSafe => this.call ??= new(this, new(), new(), this.TypeEncoding);
        private SpecialName SpecialNameSafe => this.specialName ??= new(this, this.TypeEncoding, new());

        protected override DecoratedName GenerateName()
        {
            var declaration = new DecoratedName(this);

            if (!this.TypeEncoding.Name.IsValid)
                return new(this, NodeStatus.Invalid);

            if (this.TypeEncoding.Name.IsMissing)
                return new DecoratedName(this, NodeStatus.Truncated) + this.Symbol;

            if (this.TypeEncoding.IsNone)
                return new(this, this.Symbol);

            bool needsModifiers;

            if (this.IsCall)
            {
                this.CallSafe.Declaration = declaration;
                this.CallSafe.Symbol = this.Symbol;
                declaration = this.CallSafe.Name;
                needsModifiers = this.CallSafe.NeedsModifiers;
            }
            else
            {
                this.SpecialNameSafe.Symbol = declaration.Append(this.Symbol);
                declaration = this.SpecialNameSafe.Name;
                needsModifiers = this.SpecialNameSafe.NeedsModifiers;
            }

            // reset parent
            declaration = new(this, declaration);

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
            this.typeEncoding = new(this, ref pSource);

            if (!this.TypeEncoding.Name.IsValid || this.TypeEncoding.Name.IsMissing || this.TypeEncoding.IsNone)
                return;

            if (this.IsCall)
                this.call = new Call(this, ref pSource, new(), this.Symbol,
                                     this.TypeEncoding);
            else
                this.specialName = new(this, ref pSource, this.TypeEncoding, this.Symbol);
        }

        protected override DecoratedName GenerateCode()
        {
            var code = new DecoratedName(this, this.TypeEncoding.Code);
            code.Append(this.IsCall ? this.CallSafe.Code : this.SpecialNameSafe.Code);

            return code;
        }
    }
}