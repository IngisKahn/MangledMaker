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


        protected override DecoratedName GenerateName() => new(this, this.Value);

        private unsafe void Parse(ref char* pSource)
        {
            DecoratedName result = new(this, new(pSource), this.Terminator,
                                           this.UnDecorator.DoNoIdentCharCheck);
            this.Value = result.ToString();
            pSource += result.Length;
            if (*pSource == '@')
                pSource++;
        }

        protected override DecoratedName GenerateCode() => new DecoratedName(this, this.Value) + this.Terminator;
    }
}