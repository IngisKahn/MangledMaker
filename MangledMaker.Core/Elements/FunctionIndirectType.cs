namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class FunctionIndirectType : ComplexElement
    {
        private BasedType basedType;
        private int completeLevel;
        private bool isBased;
        private bool isScoped;
        private Scope scope;
        private ThisType thisType;

        public FunctionIndirectType(ComplexElement parent, DecoratedName superType)
            : base(parent) =>
            this.SuperType = superType;

        public unsafe FunctionIndirectType(ComplexElement parent, ref char* pSource,
                                           DecoratedName superType)
            : this(parent, superType)
        {
            this.Parse(ref pSource);
        }

        [Input]
        public DecoratedName SuperType { get; set; }

        [Setting]
        public bool IsScoped
        {
            get => this.isScoped;
            set
            {
                this.isScoped = value;
                this.completeLevel = int.MaxValue;
            }
        }

        [Setting]
        public bool IsBased
        {
            get => this.isBased;
            set
            {
                this.isBased = value;
                this.completeLevel = int.MaxValue;
            }
        }

        [Child]
        public Scope? Scope => this.isScoped ? this.scope : null;

        [Child]
        public ThisType? ThisType => this.isScoped ? this.thisType : null;

        [Child]
        public BasedType? BasedType => this.isBased ? this.basedType : null;

        [Child]
        public CallingConvention CallingConvention { get; private set; }

        [Child]
        public ReturnType ReturnType { get; private set; }

        [Child]
        public ArgumentTypes ArgumentTypes { get; private set; }

        [Child]
        public ThrowTypes ThrowTypes { get; private set; }

        protected override void CreateEmptyElements()
        {
            if (this.scope == null) this.scope = new Scope(this);
            if (this.thisType == null) this.thisType = new ThisType(this);
            if (this.basedType == null) this.basedType = new BasedType(this);
            if (this.CallingConvention == null)
                this.CallingConvention = new CallingConvention(this);
            if (this.ReturnType == null) this.ReturnType = new ReturnType(this, new DecoratedName());
            if (this.ArgumentTypes == null) this.ArgumentTypes = new ArgumentTypes(this);
            if (this.ThrowTypes == null) this.ThrowTypes = new ThrowTypes(this);
        }

        protected override DecoratedName GenerateName()
        {
            if (this.completeLevel == 0)
                return new DecoratedName(this, NodeStatus.Truncated) + this.SuperType;

            var classType = new DecoratedName(this);
            var fitType = new DecoratedName(this, this.SuperType);

            if (this.isScoped)
            {
                fitType.Prepend("::");
                if (this.completeLevel > 1)
                {
                    fitType.Prepend(this.scope.Name);
                    fitType.Prepend(' ');
                }
                else
                    fitType.Prepend(NodeStatus.Truncated);
                if (this.completeLevel <= 2)
                    return NodeStatus.Truncated + fitType;
                if (this.UnDecorator.DoThisTypes)
                    classType.Assign(this.thisType.Name);
                else
                    classType.Skip(this.thisType.Name);
            }

            if (this.isBased)
                if (this.UnDecorator.DoMicrosoftKeywords)
                {
                    fitType.Prepend(this.basedType.Name);
                    fitType.Prepend(' ');
                }
                else
                    fitType.Skip(this.basedType.Name);

            if (this.UnDecorator.DoMicrosoftKeywords)
                fitType.Prepend(this.CallingConvention.Name);
            else
                fitType.Skip(this.CallingConvention.Name);
            if (!this.SuperType.IsEmpty)
                fitType.Assign('(' + fitType + ')');

            var pDeclarator = new DecoratedName(this);
            this.ReturnType.Declarator = pDeclarator;
            var returnType = this.ReturnType.Name;
            fitType.Append('(');
            fitType.Append(this.ArgumentTypes.Name);
            fitType.Append(')');
            if (this.UnDecorator.DoThisTypes && this.isScoped)
                fitType.Append(classType);
            if (this.UnDecorator.DoThrowTypes)
                fitType.Append(this.ThrowTypes.Name);
            else
                fitType.Skip(this.ThrowTypes.Name);
            pDeclarator.Assign(fitType);

            return returnType;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.completeLevel = 0;
            this.isBased = this.isScoped = false;
            switch (*pSource++)
            {
                case '\0':
                    pSource--;
                    return;
                case '6':
                case '7':
                    break;
                case '8':
                case '9':
                    this.isScoped = true;
                    break;
                case '_':
                    this.isBased = true;
                    switch (*pSource++)
                    {
                        case '\0':
                            pSource--;
                            return;
                        case 'A':
                        case 'B':
                            break;
                        case 'C':
                        case 'D':
                            this.isScoped = true;
                            break;
                        default:
                            this.IsInvalid = true;
                            return;
                    }
                    break;
                default:
                    this.IsInvalid = true;
                    return;
            }
            this.completeLevel++;

            if (this.isScoped)
            {
                if (*pSource != '\0')
                {
                    this.completeLevel++;
                    this.scope = new Scope(this, ref pSource);
                }

                if (*pSource == '\0')
                    return;

                this.completeLevel++;
                if (*pSource != '@')
                {
                    this.IsInvalid = true;
                    return;
                }
                pSource++;
                this.thisType = new ThisType(this, ref pSource);
            }

            if (this.isBased)
                this.basedType = new BasedType(this, ref pSource);

            this.CallingConvention = new CallingConvention(this, ref pSource);
            this.ReturnType = new ReturnType(this, ref pSource, new DecoratedName());
            this.ArgumentTypes = new ArgumentTypes(this, ref pSource);
            this.ThrowTypes = new ThrowTypes(this, ref pSource);
        }

        protected override DecoratedName GenerateCode()
        {
            var code = new DecoratedName(this);

            if (this.completeLevel > 0)
                if (this.isBased)
                {
                    code.Append('_');
                    code.Append(this.IsScoped ? 'C' : 'A');
                }
                else if (this.IsScoped)
                    code.Append('8');
                else
                    code.Append('6');

            if (this.isScoped)
            {
                if (this.completeLevel == 1) return code;

                code.Append(this.scope.Code);

                if (this.completeLevel == 2) return code;

                code.Append(this.thisType.Code);
            }

            if (this.isBased)
                code.Append(this.basedType.Code);

            code.Append(this.CallingConvention.Code + this.ReturnType.Code
                        + this.ArgumentTypes.Code + this.ThrowTypes.Code);

            return code;
        }
    }
}