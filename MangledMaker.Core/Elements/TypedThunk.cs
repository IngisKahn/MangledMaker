namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class TypedThunk : ComplexElement
    {
        public TypedThunk(ComplexElement parent, DecoratedName declaration, DecoratedName symbol,
                          TypeEncoding typeEncoding)
            : base(parent)
        {
            this.Symbol = symbol;
            this.TypeEncoding = typeEncoding;
            this.Declaration = declaration;
        }

        public unsafe TypedThunk(ComplexElement parent, ref char* pSource,
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

        private CallIndex? callIndex;
        [Child]
        public CallIndex CallIndex { get => this.callIndex ??= new(this); private set => this.callIndex = value; }

        private VCallThunkType? virtualCallThunkType;
        [Child]
        public VCallThunkType VirtualCallThunkType { get => this.virtualCallThunkType ??= new(this); private set => this.virtualCallThunkType = value; }

        private CallingConvention? callingConvention;
        [Child]
        public CallingConvention CallingConvention { get => this.callingConvention ??= new(this); private set => this.callingConvention = value; }

        protected override DecoratedName GenerateName()
        {
            DecoratedName thunkDeclaration = new(this, this.Declaration);
            thunkDeclaration.Append(new DecoratedName(this, this.Symbol) + '{' + this.CallIndex.Name);
            DecoratedName vCallThunkType = new(this, this.VirtualCallThunkType.Name);
            if (!this.UnDecorator.DoNameOnly)
                thunkDeclaration.Append(',' + vCallThunkType + "}' ");
            thunkDeclaration.Append("}'");
            DecoratedName callingConvention = new(this, this.CallingConvention.Name);
            if (this.UnDecorator.DoMicrosoftKeywords && this.UnDecorator.DoAllocationLanguage &&
                !this.UnDecorator.DoNameOnly)
                thunkDeclaration.Prepend(' ' + callingConvention + ' ');
            return thunkDeclaration;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.CallIndex = new(this, ref pSource);
            this.VirtualCallThunkType = new(this, ref pSource);
            this.CallingConvention = new(this, ref pSource);
        }

        protected override DecoratedName GenerateCode() => this.CallIndex.Code + this.VirtualCallThunkType.Code + this.CallingConvention.Code;
    }
}