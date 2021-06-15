namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class FunctionIndirectType : ComplexElement
    {
        private BasedType? basedType;

        private BasedType BasedTypeSafe => this.basedType ??= new(this);

        private int completeLevel;
        private bool isBased;
        private bool isScoped;

        private Scope ScopeSafe => this.scope ??= new(this);

        private Scope? scope;
        private ThisType? thisType;

        private ThisType ThisTypeSafe => this.thisType ??= new(this);


        public FunctionIndirectType(ComplexElement parent, DecoratedName superType)
            : base(parent) =>
            this.SuperType = superType;

        public unsafe FunctionIndirectType(ComplexElement parent, ref char* pSource,
                                           DecoratedName superType)
            : this(parent, superType) =>
            this.Parse(ref pSource);

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

        private CallingConvention? callingConvention;

        [Child]
        public CallingConvention CallingConvention
        {
            get => this.callingConvention ??= new(this);
            private set => this.callingConvention = value;
        }

        private ReturnType? returnType;


        [Child]
        public ReturnType ReturnType
        {
            get => this.returnType ??= new(this, new());
            private set => this.returnType = value;
        }

        private ArgumentTypes? argumentTypes;


        [Child]
        public ArgumentTypes ArgumentTypes
        {
            get => this.argumentTypes ??= new(this);
            private set => this.argumentTypes = value;
        }

        private ThrowTypes? throwTypes;


        [Child]
        public ThrowTypes ThrowTypes
        {
            get => this.throwTypes ??= new(this);
            private set => this.throwTypes = value;
        }

        protected override DecoratedName GenerateName()
        {
            if (this.completeLevel == 0)
                return new DecoratedName(this, NodeStatus.Truncated) + this.SuperType;

            DecoratedName classType = new(this);
            DecoratedName fitType = new(this, this.SuperType);

            if (this.isScoped)
            {
                fitType.Prepend("::");
                if (this.completeLevel > 1)
                {
                    fitType.Prepend(this.ScopeSafe.Name);
                    fitType.Prepend(' ');
                }
                else
                    fitType.Prepend(NodeStatus.Truncated);
                if (this.completeLevel <= 2)
                    return NodeStatus.Truncated + fitType;
                if (this.UnDecorator.DoThisTypes)
                    classType.Assign(this.ThisTypeSafe.Name);
                else
                    classType.Skip(this.ThisTypeSafe.Name);
            }

            if (this.isBased)
                if (this.UnDecorator.DoMicrosoftKeywords)
                {
                    fitType.Prepend(this.BasedTypeSafe.Name);
                    fitType.Prepend(' ');
                }
                else
                    fitType.Skip(this.BasedTypeSafe.Name);

            if (this.UnDecorator.DoMicrosoftKeywords)
                fitType.Prepend(this.CallingConvention.Name);
            else
                fitType.Skip(this.CallingConvention.Name);
            if (!this.SuperType.IsEmpty)
                fitType.Assign('(' + fitType + ')');

            DecoratedName pDeclarator = new(this);
            this.ReturnType.Declarator = pDeclarator;
            var returnTypeName = this.ReturnType.Name;
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

            return returnTypeName;
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
                    this.scope = new(this, ref pSource);
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
                this.thisType = new(this, ref pSource);
            }

            if (this.isBased)
                this.basedType = new(this, ref pSource);

            this.CallingConvention = new(this, ref pSource);
            this.ReturnType = new(this, ref pSource, new());
            this.ArgumentTypes = new(this, ref pSource);
            this.ThrowTypes = new(this, ref pSource);
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

                code.Append(this.ScopeSafe.Code);

                if (this.completeLevel == 2) return code;

                code.Append(this.ThisTypeSafe.Code);
            }

            if (this.isBased)
                code.Append(this.BasedTypeSafe.Code);

            code.Append(this.CallingConvention.Code + this.ReturnType.Code
                        + this.ArgumentTypes.Code + this.ThrowTypes.Code);

            return code;
        }
    }
}