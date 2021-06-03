namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class TerminatedName : ComplexElement
    {
        public TerminatedName(ComplexElement parent, char terminator)
            : base(parent)
        {
            this.Terminator = terminator;
            this.Value = "";
        }

        public unsafe TerminatedName(ComplexElement parent, ref char* pSource, char terminator)
            : this(parent, terminator)
        {
            this.Parse(ref pSource);
        }

        [Input]
        public char Terminator { get; private set; }

        [Setting]
        public string Value { get; set; }


        protected override DecoratedName GenerateName()
        {
            return new DecoratedName(this, this.Value);
        }

        private unsafe void Parse(ref char* pSource)
        {
            var result = new DecoratedName(this, new string(pSource), this.Terminator,
                                           this.UnDecorator.DoNoIdentCharCheck);
            this.Value = result.ToString();
            pSource += result.Length;
            if (*pSource == '@')
                pSource++;
        }

        protected override DecoratedName GenerateCode()
        {
            return new DecoratedName(this, this.Value) + this.Terminator;
        }
    }
}