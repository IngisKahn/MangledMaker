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

        private FullName? fullName;
        private FullName FullNameSafe => this.fullName ??= new(this);
        private LexicalFrame? lexicalFrame;
        private LexicalFrame LexicalFrameSafe => this.lexicalFrame ??= new(this);

        private TerminatedName? namespaceName;
        private TerminatedName NamespaceNameSafe => this.namespaceName ??= new(this, '@');
        private OperatorName? namespaceOperator;

        private OperatorName NamespaceOperatorSafe => this.namespaceOperator ??= new(this, false, true)
        { Type = OperatorName.OperatorType.Extended, ExtendedType = OperatorName.ExtendedOperatorType.Namespace };

        private ZName? zName;
        private ZName ZNameSafe => this.zName ??= new(this, true);

        public Scope(ComplexElement parent) : base(parent)
        { }

        public unsafe Scope(ComplexElement parent, ref char* pSource)
            : base(parent) =>
            this.Parse(ref pSource);

        [Setting]
        public ScopeMode Mode { get; set; }

        private Scope? outsideScope;
        [Child]
        public Scope OutsideScope { get => this.outsideScope ??= new(this); set => this.outsideScope = value; }

        [Child]
        public ZName? ZName
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
        public OperatorName? NamespaceOperator => this.Mode == ScopeMode.NamespaceOperator ? this.namespaceOperator : null;

        [Child]
        public FullName? FullName => this.Mode == ScopeMode.FullName ? this.fullName : null;

        [Child]
        public TerminatedName? NamespaceName => this.Mode == ScopeMode.Namespace ? this.namespaceName : null;

        [Child]
        public LexicalFrame? LexicalFrame => this.Mode == ScopeMode.LexicalFrame ? this.lexicalFrame : null;

        protected override DecoratedName GenerateName()
        {
            DecoratedName scope = new (this);
            if (this.UnDecorator.ExplicitTemplateParams && !this.UnDecorator.CanGetTemplateArgumentList)
                return scope;

            switch (this.Mode)
            {
                case ScopeMode.None:
                    return scope;
                case ScopeMode.ZName:
                    scope.Assign(this.ZNameSafe.Name);
                    break;
                case ScopeMode.FullName:
                    scope.Assign('`');
                    scope += this.FullNameSafe.Name;
                    scope += '\'';
                    break;
                case ScopeMode.NamespaceOperator:
                    scope.Assign(this.NamespaceOperatorSafe.Name);
                    break;
                case ScopeMode.BracketedName:
                    scope.Assign('[');
                    scope += this.ZNameSafe.Name;
                    scope += ']';
                    break;
                case ScopeMode.Namespace:
                    //if (!this.UnDecorator.ZNameList.IsFull)
                    //    this.UnDecorator.ZNameList.Append(this.namespaceName.Name);
                    scope.Assign("`anonymous namespace'");
                    break;
                default:
                    scope.Assign(this.LexicalFrameSafe.Name);
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
                            this.zName = new(this, ref pSource, true);
                            break;
                        case '%':
                        case 'A':
                            // It an anonymous namespace, skip the (unreadable) name and instead insert
                            // an appropriate string
                            this.Mode = ScopeMode.Namespace;
                            this.namespaceName = new(this, ref pSource, '@');

                            if (!this.UnDecorator.ZNameList.IsFull)
                                this.UnDecorator.ZNameList.Append(this.namespaceName);
                            break;
                        case '?':
                            if (pSource[1] == '_' && pSource[2] == '?')
                            {
                                this.Mode = ScopeMode.NamespaceOperator;
                                pSource++;
                                this.namespaceOperator = new(this, ref pSource, false,
                                    true);

                                if (*pSource == '@')
                                    pSource++;
                            }
                            else
                            {
                                this.Mode = ScopeMode.FullName;
                                this.fullName = new(this, ref pSource);
                            }
                            break;
                        case 'I':
                            this.Mode = ScopeMode.BracketedName;
                            pSource++;
                            this.zName = new(this, ref pSource, true);
                            break;
                        default:
                            this.Mode = ScopeMode.LexicalFrame;
                            this.lexicalFrame = new(this, ref pSource); // Skip lexical scope info
                            break;
                    }
                else
                {
                    this.Mode = ScopeMode.ZName;
                    this.zName = new(this, ref pSource, true);
                }
            }

            if (*pSource != '\0' && *pSource != '@')
                this.OutsideScope = new(this, ref pSource);

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
            DecoratedName scope = new(this, '?');

            switch (this.Mode)
            {
                case ScopeMode.None:
                    return new(this, '@');
                case ScopeMode.ZName:
                    scope = new(this, this.ZNameSafe.Code);
                    break;
                case ScopeMode.FullName:
                    scope += this.FullNameSafe.Code;
                    break;
                case ScopeMode.NamespaceOperator:
                    scope += '?';
                    scope += this.NamespaceOperatorSafe.Code;
                    break;
                case ScopeMode.BracketedName:
                    scope += 'I';
                    scope += this.ZNameSafe.Code;
                    break;
                case ScopeMode.Namespace:
                    scope += 'A';
                    scope += this.NamespaceNameSafe.Code;
                    break;
            }

            if (this.OutsideScope.Mode != ScopeMode.None)
                scope += this.OutsideScope.Code;

            return scope + '@';
        }
    }
}