namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class Scope : ComplexElement
    {
        public enum ScopeMode
        {
            None,
            ZName,
            Namespace,
            NamespaceOperator,
            FullName,
            BracketedName,
            LexicalFrame
        }

        private FullName fullName;
        private LexicalFrame lexicalFrame;

        private TerminatedName namespaceName;
        private OperatorName namespaceOperator;

        private ZName zName;

        public Scope(ComplexElement parent) : base(parent)
        { }

        public unsafe Scope(ComplexElement parent, ref char* pSource)
            : base(parent)
        {
            this.Parse(ref pSource);
        }

        [Setting]
        public ScopeMode Mode { get; set; }

        [Child]
        public Scope OutsideScope { get; set; }

        [Child]
        public ZName ZName
        {
            get
            {
                switch (this.Mode)
                {
                    case ScopeMode.ZName:
                    case ScopeMode.BracketedName:
                        return this.zName;
                    default:
                        return null;
                }
            }
        }

        [Child]
        public OperatorName NamespaceOperator
        {
            get { return this.Mode == ScopeMode.NamespaceOperator ? this.namespaceOperator : null; }
        }

        [Child]
        public FullName FullName
        {
            get { return this.Mode == ScopeMode.FullName ? this.fullName : null; }
        }

        [Child]
        public TerminatedName NamespaceName
        {
            get { return this.Mode == ScopeMode.Namespace ? this.namespaceName : null; }
        }

        [Child]
        public LexicalFrame LexicalFrame
        {
            get { return this.Mode == ScopeMode.LexicalFrame ? this.lexicalFrame : null; }
        }

        protected override void CreateEmptyElements()
        {
            if (this.OutsideScope == null) this.OutsideScope = new Scope(this);
            if (this.zName == null) this.zName = new ZName(this, true);
            if (this.fullName == null) this.fullName = new FullName(this);
            if (this.namespaceOperator == null)
            {
                this.namespaceOperator = new OperatorName(this, false, true)
                    {
                        Type = OperatorName.OperatorType.Extended,
                        ExtendedType = OperatorName.ExtendedOperatorType.Namespace
                    };
            }
            if (this.namespaceName == null) this.namespaceName = new TerminatedName(this, '@');
            if (this.lexicalFrame == null) this.lexicalFrame = new LexicalFrame(this);
        }

        protected override DecoratedName GenerateName()
        {
            var scope = new DecoratedName(this);
            if (this.UnDecorator.ExplicitTemplateParams && !this.UnDecorator.CanGetTemplateArgumentList)
                return scope;

            switch (this.Mode)
            {
                case ScopeMode.None:
                    return scope;
                case ScopeMode.ZName:
                    scope.Assign(this.zName.Name);
                    break;
                case ScopeMode.FullName:
                    scope.Assign('`');
                    scope += this.fullName.Name;
                    scope += '\'';
                    break;
                case ScopeMode.NamespaceOperator:
                    scope.Assign(this.namespaceOperator.Name);
                    break;
                case ScopeMode.BracketedName:
                    scope.Assign('[');
                    scope += this.zName.Name;
                    scope += ']';
                    break;
                case ScopeMode.Namespace:
                    //if (!this.UnDecorator.ZNameList.IsFull)
                    //    this.UnDecorator.ZNameList.Append(this.namespaceName.Name);
                    scope.Assign("`anonymous namespace'");
                    break;
                default:
                    scope.Assign(this.lexicalFrame.Name);
                    break;
            }

            if (this.OutsideScope.Name.IsEmpty) 
                return scope;
            scope.Prepend("::");
            scope.Prepend(this.OutsideScope.Name);

            return scope;
        }

        private unsafe void Parse(ref char* pSource)
        {
            //	Get the list of scopes
            if (*pSource != '\0' && *pSource != '@')
            {
                if (this.UnDecorator.ExplicitTemplateParams && !this.UnDecorator.CanGetTemplateArgumentList)
                    return;

                if (*pSource == '?')
                    switch (*++pSource)
                    {
                        case '$':
                            // It's a template name, which is a kind of zname; back up
                            // and handle like a zname.
                            this.Mode = ScopeMode.ZName;
                            pSource--;
                            this.zName = new ZName(this, ref pSource, true);
                            break;
                        case '%':
                        case 'A':
                            // It an anonymous namespace, skip the (unreadable) name and instead insert
                            // an appropriate string
                            this.Mode = ScopeMode.Namespace;
                            this.namespaceName = new TerminatedName(this, ref pSource, '@');

                            if (!this.UnDecorator.ZNameList.IsFull)
                                this.UnDecorator.ZNameList.Append(this.namespaceName);
                            break;
                        case '?':
                            if (pSource[1] == '_' && pSource[2] == '?')
                            {
                                this.Mode = ScopeMode.NamespaceOperator;
                                pSource++;
                                this.namespaceOperator = new OperatorName(this, ref pSource, false,
                                    true);

                                if (*pSource == '@')
                                    pSource++;
                            }
                            else
                            {
                                this.Mode = ScopeMode.FullName;
                                this.fullName = new FullName(this, ref pSource);
                            }
                            break;
                        case 'I':
                            this.Mode = ScopeMode.BracketedName;
                            pSource++;
                            this.zName = new ZName(this, ref pSource, true);
                            break;
                        default:
                            this.Mode = ScopeMode.LexicalFrame;
                            this.lexicalFrame = new LexicalFrame(this, ref pSource); // Skip lexical scope info
                            break;
                    }
                else
                {
                    this.Mode = ScopeMode.ZName;
                    this.zName = new ZName(this, ref pSource, true);
                }
            }

            if (*pSource != '\0' && *pSource != '@')
                this.OutsideScope = new Scope(this, ref pSource);

            switch (*pSource)
            {
                case '\0':
                    this.IsTruncated = true;
                    break;
                case '@':
                    break;
                default:
                    this.IsInvalid = true;
                    break;
            }
        }

        protected override DecoratedName GenerateCode()
        {
            var scope = new DecoratedName(this, '?');

            switch (this.Mode)
            {
                case ScopeMode.None:
                    return new DecoratedName(this, '@');
                case ScopeMode.ZName:
                    scope = new DecoratedName(this, this.zName.Code);
                    break;
                case ScopeMode.FullName:
                    scope += this.fullName.Code;
                    break;
                case ScopeMode.NamespaceOperator:
                    scope += '?';
                    scope += this.namespaceOperator.Code;
                    break;
                case ScopeMode.BracketedName:
                    scope += 'I';
                    scope += this.zName.Code;
                    break;
                case ScopeMode.Namespace:
                    scope += 'A';
                    scope += this.namespaceName.Code;
                    break;
            }

            if (this.OutsideScope.Mode != ScopeMode.None)
                scope += this.OutsideScope.Code;

            return scope + '@';
        }
    }
}