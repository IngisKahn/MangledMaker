namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class FullName : ComplexElement
    {
        private DataType dataName;
        private Declaration declaration;
        private bool isSubName;
        private Scope parentScope;

        private FullName subName;

        private SymbolName symbolName;
        private Scope templateScope;

        public FullName(ComplexElement parent) : base(parent)
        { }

        public unsafe FullName(ComplexElement parent, ref char* pSource)
            : this(parent)
        {
            this.Parse(ref pSource);
        }

        [Child]
        public DataType DataName
        {
            get { return this.UnDecorator.DoTypeOnly ? this.dataName : null; }
        }

        [Child]
        public FullName SubName
        {
            get { return !this.UnDecorator.DoTypeOnly && this.isSubName ? this.subName : null; }
        }

        [Child]
        public SymbolName SymbolName
        {
            get { return !this.UnDecorator.DoTypeOnly && !this.isSubName ? this.symbolName : null; }
        }

        [Child]
        public Scope ParentScope
        {
            get { return !this.UnDecorator.DoTypeOnly && !this.isSubName ? this.parentScope : null; }
        }

        [Child]
        public Scope TemplateScope
        {
            get
            {
                return !this.UnDecorator.DoTypeOnly && !this.isSubName
                       && this.UnDecorator.ExplicitTemplateParams
                           ? this.templateScope
                           : null;
            }
        }

        [Child]
        public Declaration Declaration
        {
            get { return !this.UnDecorator.DoTypeOnly && !this.isSubName ? this.declaration : null; }
        }

        [Setting]
        public bool? IsSubName
        {
            get { return this.UnDecorator.DoTypeOnly ? (bool?)null : this.isSubName; }
            set { if (value != null) this.isSubName = (bool)value; }
        }

        protected override void CreateEmptyElements()
        {
            if (this.dataName == null) 
                this.dataName = new DataType(this, new DecoratedName(this));
            if (this.subName == null) 
                this.subName = new FullName(this);
            if (this.symbolName == null) 
                this.symbolName = new SymbolName(this);
            if (this.parentScope == null) 
                this.parentScope = new Scope(this);
            if (this.templateScope == null) 
                this.templateScope = new Scope(this);
            if (this.declaration == null)
                this.declaration = new Declaration(this, new DecoratedName(this));
        }

        protected override DecoratedName GenerateName()
        {
            if (this.UnDecorator.DoTypeOnly)
                return this.dataName.Name;
            if (this.isSubName)
                return this.subName.Name;
            var sn = this.symbolName.Name;

            var isSymbolUdc = sn.IsUdc;
            var isSymbolVirtualThunk = sn.IsVirtualCallThunk;
            if (!sn.IsValid)
                return sn;
            var s = this.parentScope.Name;
            if (!s.IsEmpty)
                if (this.UnDecorator.ExplicitTemplateParams)
                {
                    this.UnDecorator.ExplicitTemplateParams = false;
                    sn.Assign(sn + s);
                    if (!this.templateScope.Name.IsEmpty)
                    {
                        s.Assign(this.templateScope.Name);
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
            this.declaration.Symbol = sn;
            return this.declaration.Name;
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
                this.dataName = new DataType(this, ref pSource, new DecoratedName(this));
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
                        this.subName = new FullName(this, ref pSource);
                        while (*pSource != '\0') pSource++;
                    }
                    else
                    {
                        this.symbolName = new SymbolName(this, ref pSource);
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

                        if (this.UnDecorator.DoNameOnly && !isSymbolUdc && !sn.IsVirtualCallThunk)
                            this.declaration = new Declaration(this, ref pSource,
                                new DecoratedName(this));
                        else
                            this.declaration = new Declaration(this, ref pSource, sn);
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
                return this.dataName.Code;

            var result = new DecoratedName(this, '?');
            if (this.isSubName)
                return result + this.subName.Code;
            result += this.symbolName.Code;
            if (!this.parentScope.Name.IsEmpty)
            {
                result += this.parentScope.Code;
                if (this.UnDecorator.ExplicitTemplateParams && !this.templateScope.Name.IsEmpty)
                    result += this.templateScope.Code;
            }
            result += '@';
            if (!this.symbolName.Name.IsEmpty)
            {
                //declaration.Symbol = 
                result += this.declaration.Code;
            }

            return result;
        }
    }
}