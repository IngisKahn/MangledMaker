namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class Function : ComplexElement
    {
        private Displacement? displacement1;
        private Displacement Displacement1Safe => this.displacement1 ??= new(this);
        private Displacement? displacement2;
        private Displacement Displacement2Safe => this.displacement2 ??= new(this);
        private Displacement? displacement3;
        private Displacement Displacement3Safe => this.displacement3 ??= new(this);
        private Displacement? displacement4;
        private Displacement Displacement4Safe => this.displacement4 ??= new(this);
        private ThisType? thisType;
        private ThisType ThisTypeSafe => this.thisType ??= new(this);

        public Function(ComplexElement parent, DecoratedName declaration, DecoratedName symbol,
                        TypeEncoding typeEncoding)
            : base(parent)
        {
            this.Symbol = symbol;
            this.TypeEncoding = typeEncoding;
            this.Declaration = declaration;
        }

        public unsafe Function(ComplexElement parent, ref char* pSource,
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
        public Displacement? Displacement1 => this.TypeEncoding.IsThunk ? this.displacement1 : null;

        [Child]
        public Displacement? Displacement2 =>
            this.TypeEncoding.IsThunk &&
            (this.TypeEncoding.IsVirtualConstructorExtended || this.TypeEncoding.IsVirtualConstructor)
                ? this.displacement2
                : null;

        [Child]
        public Displacement? Displacement3 =>
            this.TypeEncoding.IsThunk && this.TypeEncoding.IsVirtualConstructorExtended
                ? this.displacement3
                : null;

        [Child]
        public Displacement? Displacement4 =>
            this.TypeEncoding.IsThunk && this.TypeEncoding.IsVirtualConstructorExtended
                ? this.displacement4
                : null;

        [Child]
        public ThisType? ThisType => !this.TypeEncoding.IsMemberStatic ? this.thisType : null;

        private CallingConvention? callingConvention;

        [Child] public CallingConvention CallingConvention => this.callingConvention ??= new(this);

        private ReturnType? returnType;

        [Child] public ReturnType ReturnType => this.returnType ??= new(this, new(this));

        private ArgumentTypes? argumentTypes;

        [Child] public ArgumentTypes ArgumentTypes => this.argumentTypes ??= new(this);

        private ThrowTypes? throwTypes;

        [Child] public ThrowTypes ThrowTypes => this.throwTypes ??= new(this);

        [Output]
        public bool NeedsModifiers { get; private set; }

        protected override DecoratedName GenerateName()
        {
            var declaration = new DecoratedName(this, this.Declaration);

            DecoratedName disp4 = new(this),
                          disp3 = new(this),
                          disp2 = new(this),
                          disp1 = new(this),
                          classType = new(this);

            if (this.TypeEncoding.IsThunk)
            {
                if (this.TypeEncoding.IsVirtualConstructorExtended)
                {
                    disp4.Assign(this.Displacement4Safe.Name);
                    disp3.Assign(this.Displacement3Safe.Name);
                    disp2.Assign(this.Displacement2Safe.Name);
                }
                else if (this.TypeEncoding.IsVirtualConstructor)
                    disp2.Assign(this.Displacement2Safe.Name);

                disp1.Assign(this.Displacement1Safe.Name);
            }

            if (this.TypeEncoding.IsClass && !this.TypeEncoding.IsMemberStatic)
                if (this.UnDecorator.DoThisTypes)
                    classType.Assign(this.ThisTypeSafe.Name);
                else
                    classType.Skip(this.ThisTypeSafe.Name);

            if (this.UnDecorator.DoMicrosoftKeywords && this.UnDecorator.DoAllocationLanguage)
                declaration.Prepend(this.CallingConvention.Name);
            else
                declaration.Skip(this.CallingConvention.Name);

            if (!this.Symbol.IsEmpty)
                if (!(declaration.IsEmpty || this.UnDecorator.DoNameOnly))
                    declaration.Append(' ' + new DecoratedName(this, this.Symbol));
                else
                    declaration.Assign(this.Symbol);

            DecoratedName? holder = null;
            var linkedReturnType = new DecoratedName(this);
            if (this.Symbol.IsUdc)
            {
                this.ReturnType.Declarator = null;
                declaration.Append(' ' + new DecoratedName(this, this.ReturnType.Name));
                if (this.UnDecorator.DoNameOnly)
                {
                    this.NeedsModifiers = false;
                    return declaration;
                }
            }
            else
            {
                this.ReturnType.Declarator = holder = new(this);
                linkedReturnType.Assign(this.ReturnType.Name);
            }
            if (this.TypeEncoding.IsThunk)
            {
                if (this.TypeEncoding.IsVirtualConstructorExtended)
                    declaration.Append(new DecoratedName(this, "`vtordispex{") + disp4 +
                                       ',' + disp3 + ',' + disp2 + ',');
                else if (this.TypeEncoding.IsVirtualConstructor)
                    declaration.Append(new DecoratedName(this, "`vtordisp{") + disp2 + ',');
                else
                    declaration.Append("`adjustor{");
                declaration.Append(disp1 + new DecoratedName(this, "}' "));
            }

            declaration.Append(new DecoratedName(this, '(') + this.ArgumentTypes.Name + ')');

            if (this.TypeEncoding.IsClass && !this.TypeEncoding.IsMemberStatic)
                declaration.Append(classType);

            if (this.UnDecorator.DoThrowTypes)
                declaration.Append(this.ThrowTypes.Name);
            else
                declaration.Skip(this.ThrowTypes.Name);

            if (this.UnDecorator.DoFunctionReturns && holder != null)
            {
                holder.Assign(declaration);
                declaration.Assign(linkedReturnType);
            }

            this.NeedsModifiers = true;

            return declaration;
        }

        private unsafe void Parse(ref char* pSource)
        {
            if (this.TypeEncoding.IsThunk)
            {
                if (this.TypeEncoding.IsVirtualConstructorExtended)
                {
                    this.displacement4 = new(this, ref pSource);
                    this.displacement3 = new(this, ref pSource);
                    this.displacement2 = new(this, ref pSource);
                }
                else if (this.TypeEncoding.IsVirtualConstructor)
                    this.displacement2 = new(this, ref pSource);

                this.displacement1 = new(this, ref pSource);
            }

            if (this.TypeEncoding.IsClass && !this.TypeEncoding.IsMemberStatic)
                this.thisType = new(this, ref pSource);

            this.callingConvention = new(this, ref pSource);

            this.returnType = new(this, ref pSource, this.Symbol);

            this.argumentTypes = new(this, ref pSource);

            this.throwTypes = new(this, ref pSource);
        }

        protected override DecoratedName GenerateCode()
        {
            DecoratedName code = new(this);

            if (this.TypeEncoding.IsThunk)
            {
                if (this.TypeEncoding.IsVirtualConstructorExtended)
                    code.Assign(this.Displacement4Safe.Code + this.Displacement3Safe.Code + this.Displacement2Safe.Code);
                else if (this.TypeEncoding.IsVirtualConstructor)
                    code.Assign(this.Displacement2Safe.Code);

                code.Append(this.Displacement1Safe.Code);
            }

            if (this.TypeEncoding.IsClass && !this.TypeEncoding.IsMemberStatic)
                code.Append(this.ThisTypeSafe.Code);

            return code + this.CallingConvention.Code + this.ReturnType.Code + this.ArgumentTypes.Code +
                   this.ThrowTypes.Code;
        }
    }
}