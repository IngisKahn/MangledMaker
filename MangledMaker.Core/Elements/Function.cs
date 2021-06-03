namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class Function : ComplexElement
    {
        private Displacement displacement1;
        private Displacement displacement2;
        private Displacement displacement3;
        private Displacement displacement4;
        private ThisType thisType;

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
        public Displacement Displacement1
        {
            get { return this.TypeEncoding.IsThunk ? this.displacement1 : null; }
        }

        [Child]
        public Displacement Displacement2
        {
            get
            {
                return this.TypeEncoding.IsThunk &&
                       (this.TypeEncoding.IsVirtualConstructorExtended || this.TypeEncoding.IsVirtualConstructor)
                           ? this.displacement2
                           : null;
            }
        }

        [Child]
        public Displacement Displacement3
        {
            get
            {
                return this.TypeEncoding.IsThunk && this.TypeEncoding.IsVirtualConstructorExtended
                           ? this.displacement3
                           : null;
            }
        }

        [Child]
        public Displacement Displacement4
        {
            get
            {
                return this.TypeEncoding.IsThunk && this.TypeEncoding.IsVirtualConstructorExtended
                           ? this.displacement4
                           : null;
            }
        }

        [Child]
        public ThisType ThisType
        {
            get { return !this.TypeEncoding.IsMemberStatic ? this.thisType : null; }
        }

        [Child]
        public CallingConvention CallingConvention { get; private set; }

        [Child]
        public ReturnType ReturnType { get; private set; }

        [Child]
        public ArgumentTypes ArgumentTypes { get; private set; }

        [Child]
        public ThrowTypes ThrowTypes { get; private set; }

        [Output]
        public bool NeedsModifiers { get; private set; }

        protected override void CreateEmptyElements()
        {
            if (this.displacement1 == null)
                this.displacement1 = new Displacement(this);
            if (this.displacement2 == null)
                this.displacement2 = new Displacement(this);
            if (this.displacement3 == null)
                this.displacement3 = new Displacement(this);
            if (this.displacement4 == null)
                this.displacement4 = new Displacement(this);
            if (this.thisType == null)
                this.thisType = new ThisType(this);
            if (this.CallingConvention == null)
                this.CallingConvention = new CallingConvention(this);
            if (this.ReturnType == null)
                this.ReturnType = new ReturnType(this, this.Symbol);
            if (this.ArgumentTypes == null)
                this.ArgumentTypes = new ArgumentTypes(this);
            if (this.ThrowTypes == null)
                this.ThrowTypes = new ThrowTypes(this);
        }

        protected override DecoratedName GenerateName()
        {
            var declaration = new DecoratedName(this, this.Declaration);

            DecoratedName disp4 = new DecoratedName(this),
                          disp3 = new DecoratedName(this),
                          disp2 = new DecoratedName(this),
                          disp1 = new DecoratedName(this),
                          classType = new DecoratedName(this);

            if (this.TypeEncoding.IsThunk)
            {
                if (this.TypeEncoding.IsVirtualConstructorExtended)
                {
                    disp4.Assign(this.displacement4.Name);
                    disp3.Assign(this.displacement3.Name);
                    disp2.Assign(this.displacement2.Name);
                }
                else if (this.TypeEncoding.IsVirtualConstructor)
                    disp2.Assign(this.displacement2.Name);

                disp1.Assign(this.displacement1.Name);
            }

            if (this.TypeEncoding.IsClass && !this.TypeEncoding.IsMemberStatic)
                if (this.UnDecorator.DoThisTypes)
                    classType.Assign(this.thisType.Name);
                else
                    classType.Skip(this.thisType.Name);

            if (this.UnDecorator.DoMicrosoftKeywords && this.UnDecorator.DoAllocationLanguage)
                declaration.Prepend(this.CallingConvention.Name);
            else
                declaration.Skip(this.CallingConvention.Name);

            if (!this.Symbol.IsEmpty)
                if (!(declaration.IsEmpty || this.UnDecorator.DoNameOnly))
                    declaration.Append(' ' + new DecoratedName(this, this.Symbol));
                else
                    declaration.Assign(this.Symbol);

            DecoratedName holder = null;
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
                this.ReturnType.Declarator = holder = new DecoratedName(this);
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

            if (this.UnDecorator.DoFunctionReturns && (holder != null))
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
                    this.displacement4 = new Displacement(this, ref pSource);
                    this.displacement3 = new Displacement(this, ref pSource);
                    this.displacement2 = new Displacement(this, ref pSource);
                }
                else if (this.TypeEncoding.IsVirtualConstructor)
                    this.displacement2 = new Displacement(this, ref pSource);

                this.displacement1 = new Displacement(this, ref pSource);
            }

            if (this.TypeEncoding.IsClass && !this.TypeEncoding.IsMemberStatic)
                this.thisType = new ThisType(this, ref pSource);

            this.CallingConvention = new CallingConvention(this, ref pSource);

            this.ReturnType = new ReturnType(this, ref pSource, this.Symbol);

            this.ArgumentTypes = new ArgumentTypes(this, ref pSource);

            this.ThrowTypes = new ThrowTypes(this, ref pSource);
        }

        protected override DecoratedName GenerateCode()
        {
            var code = new DecoratedName(this);

            if (this.TypeEncoding.IsThunk)
            {
                if (this.TypeEncoding.IsVirtualConstructorExtended)
                    code.Assign(this.displacement4.Code + this.displacement3.Code + this.displacement2.Code);
                else if (this.TypeEncoding.IsVirtualConstructor)
                    code.Assign(this.displacement2.Code);

                code.Append(this.displacement1.Code);
            }

            if (this.TypeEncoding.IsClass && !this.TypeEncoding.IsMemberStatic)
                code.Append(this.thisType.Code);

            return code + this.CallingConvention.Code + this.ReturnType.Code + this.ArgumentTypes.Code +
                   this.ThrowTypes.Code;
        }
    }
}