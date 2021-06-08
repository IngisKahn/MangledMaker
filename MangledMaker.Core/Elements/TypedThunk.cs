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

        [Child]
        public CallIndex CallIndex { get; private set; }

        [Child]
        public VCallThunkType VirtualCallThunkType { get; private set; }

        [Child]
        public CallingConvention CallingConvention { get; private set; }

        protected override void CreateEmptyElements()
        {
            if (this.CallIndex == null)
                this.CallIndex = new CallIndex(this);
            if (this.VirtualCallThunkType == null)
                this.VirtualCallThunkType = new VCallThunkType(this);
            if (this.CallingConvention == null)
                this.CallingConvention = new CallingConvention(this);
        }

        protected override DecoratedName GenerateName()
        {
            var thunkDeclaration = new DecoratedName(this, this.Declaration);
            thunkDeclaration.Append(new DecoratedName(this, this.Symbol) + '{' + this.CallIndex.Name);
            var vCallThunkType = new DecoratedName(this, this.VirtualCallThunkType.Name);
            if (!this.UnDecorator.DoNameOnly)
                thunkDeclaration.Append(',' + vCallThunkType + "}' ");
            thunkDeclaration.Append("}'");
            var callingConvention = new DecoratedName(this, this.CallingConvention.Name);
            if (this.UnDecorator.DoMicrosoftKeywords && this.UnDecorator.DoAllocationLanguage &&
                !this.UnDecorator.DoNameOnly)
                thunkDeclaration.Prepend(' ' + callingConvention + ' ');
            return thunkDeclaration;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.CallIndex = new CallIndex(this, ref pSource);
            this.VirtualCallThunkType = new VCallThunkType(this, ref pSource);
            this.CallingConvention = new CallingConvention(this, ref pSource);
        }

        protected override DecoratedName GenerateCode()
        {
            return this.CallIndex.Code + this.VirtualCallThunkType.Code + this.CallingConvention.Code;
        }
    }
}