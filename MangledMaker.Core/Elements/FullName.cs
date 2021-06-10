namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class FullName : ComplexElement
    {
        private DataType? dataName;
        private DataType DataNameSafe => this.dataName ??= new(this, new(this));
        private Declaration? declaration;
        private Declaration DeclarationSafe => this.declaration ??= new(this, new(this));
        private bool isSubName;
        private Scope? parentScope;
        private Scope ParentScopeSafe => this.parentScope ??= new(this);

        private FullName? subName;
        private FullName SubNameSafe => this.subName ??= new(this);

        private SymbolName? symbolName;
        private SymbolName SymbolNameSafe => this.symbolName ??= new(this);
        private Scope? templateScope;
        private Scope TemplateScopeSafe => this.templateScope ??= new(this);

        public FullName(ComplexElement parent) : base(parent)
        { }

        public unsafe FullName(ComplexElement parent, ref char* pSource)
            : this(parent) =>
            this.Parse(ref pSource);

        [Child]
        public DataType? DataName => this.UnDecorator.DoTypeOnly ? this.dataName : null;

        [Child]
        public FullName? SubName => !this.UnDecorator.DoTypeOnly && this.isSubName ? this.subName : null;

        [Child]
        public SymbolName? SymbolName => !this.UnDecorator.DoTypeOnly && !this.isSubName ? this.symbolName : null;

        [Child]
        public Scope? ParentScope => !this.UnDecorator.DoTypeOnly && !this.isSubName ? this.parentScope : null;

        [Child]
        public Scope? TemplateScope =>
            !this.UnDecorator.DoTypeOnly && !this.isSubName
                                         && this.UnDecorator.ExplicitTemplateParams
                ? this.templateScope
                : null;

        [Child]
        public Declaration? Declaration => !this.UnDecorator.DoTypeOnly && !this.isSubName ? this.declaration : null;

        [Setting]
        public bool? IsSubName
        {
            get => this.UnDecorator.DoTypeOnly ? null : this.isSubName;
            set { if (value != null) this.isSubName = (bool)value; }
        }

        protected override DecoratedName GenerateName()
        {
            if (this.UnDecorator.DoTypeOnly)
                return this.DataNameSafe.Name;
            if (this.isSubName)
                return this.SubNameSafe.Name;
            var sn = this.SymbolNameSafe.Name;

            var isSymbolUdc = sn.IsUdc;
            var isSymbolVirtualThunk = sn.IsVirtualCallThunk;
            if (!sn.IsValid)
                return sn;
            var s = this.ParentScopeSafe.Name;
            if (!s.IsEmpty)
                if (this.UnDecorator.ExplicitTemplateParams)
                {
                    this.UnDecorator.ExplicitTemplateParams = false;
                    sn.Assign(sn + s);
                    if (!this.TemplateScopeSafe.Name.IsEmpty)
                    {
                        s.Assign(this.TemplateScopeSafe.Name);
                        sn.Assign(s + new DecoratedName(this, "::") + sn);
                    }
                }
                else
                    sn.Assign(s + new DecoratedName(this, "::") + sn);
            if (isSymbolUdc)
                sn.IsUdc = true;
            if (isSymbolVirtualThunk)
                sn.IsVirtualCallThunk = true;
            if (sn.IsEmpty || sn.IsNoTypeEncoding)
                return sn;

            if (this.UnDecorator.DoNameOnly && !isSymbolUdc && !sn.IsVirtualCallThunk)
                return sn;
            this.DeclarationSafe.Symbol = sn;
            return this.DeclarationSafe.Name;
        }

        private unsafe void Parse(ref char* pSource)
        {
            //	Ensure that it is intended to be a decorated name
            if (this.UnDecorator.DoTypeOnly)
            {
                // Disable the type-only flag, so that if we get here recursively, eg.
                // in a template tag, we do full name undecoration.
                this.UnDecorator.DisableFlags &= ~DisableOptions.NoArguments;
                // If we're decoding just a type, process it as the type for an abstract
                // declarator, by giving an empty symbol name.
                this.dataName = new(this, ref pSource, new(this));
                this.UnDecorator.DisableFlags |= DisableOptions.NoArguments;
            }
            else
                //	Extract the basic symbol name
                if (*pSource == '?')
                {
                    pSource++; // Advance the original name pointer
                    if (*pSource == '?' && pSource[1] == '?')
                    {
                        this.isSubName = true;
                        this.subName = new(this, ref pSource);
                        while (*pSource != '\0') pSource++;
                    }
                    else
                    {
                        this.symbolName = new(this, ref pSource);
                        var sn = this.symbolName.Name;
                        var isSymbolUdc = sn.IsUdc;
                        var isSymbolVirtualThunk = sn.IsVirtualCallThunk;
                        //	Abort if the symbol name is invalid
                        if (!sn.IsValid)
                            return;
                        //	Extract, and prefix the scope qualifiers
                        if (*pSource != '\0' && *pSource != '@')
                        {
                            this.parentScope = new Scope(this, ref pSource);
                            var s = this.parentScope.Name;
                            if (!s.IsEmpty)
                                if (this.UnDecorator.ExplicitTemplateParams)
                                {
                                    this.UnDecorator.ExplicitTemplateParams = false;
                                    sn.Assign(sn + s);
                                    if (*pSource != '@')
                                    {
                                        this.templateScope = new Scope(this, ref pSource);
                                        s.Assign(this.templateScope.Name);
                                        sn.Assign(s + "::" + sn);
                                    }
                                }
                                else
                                    sn.Assign(s + "::" + sn);
                        }
                        if (isSymbolUdc)
                            sn.IsUdc = true;
                        //	Now compose declaration
                        if (isSymbolVirtualThunk)
                            sn.IsVirtualCallThunk = true;
                        if (sn.IsEmpty || sn.IsNoTypeEncoding)
                            return;

                        if (*pSource != '\0')
                        {
                            if (*pSource != '@')
                            {
                                this.IsInvalid = true;
                                return; // new DecoratedName(NodeStatus.Invalid);
                            }
                            pSource++;
                        }

                        this.declaration = this.UnDecorator.DoNameOnly && !isSymbolUdc && !sn.IsVirtualCallThunk
                            ? new(this, ref pSource,
                                new(this))
                            : new(this, ref pSource, sn);
                    }
                }
                else if (*pSource != '\0')
                    this.IsInvalid = true;
                else
                    this.IsTruncated = true;
        }

        protected override DecoratedName GenerateCode()
        {
            if (this.UnDecorator.DoTypeOnly)
                return this.DataNameSafe.Code;

            DecoratedName result = new(this, '?');
            if (this.isSubName)
                return result + this.SubNameSafe.Code;
            result += this.SymbolNameSafe.Code;
            if (!this.ParentScopeSafe.Name.IsEmpty)
            {
                result += this.ParentScopeSafe.Code;
                if (this.UnDecorator.ExplicitTemplateParams && !this.TemplateScopeSafe.Name.IsEmpty)
                    result += this.TemplateScopeSafe.Code;
            }
            result += '@';
            if (!this.SymbolNameSafe.Name.IsEmpty)
            //declaration.Symbol = 
                result += this.DeclarationSafe.Code;

            return result;
        }
    }
}