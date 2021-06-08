namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class CallingConvention : ComplexElement
    {
        public enum ConventionType
        {
            C,
            Pascal,
            This,
            Standard,
            Fast,
            CommonLanguageRuntime
        }

        [Setting]
        public ConventionType Convention { get; set; }

        public CallingConvention(ComplexElement parent) : base(parent)
        { }
        public unsafe CallingConvention(ComplexElement parent, ref char* pSource)
            : base(parent) =>
            this.Parse(ref pSource);

        protected override DecoratedName GenerateName()
        {
            var result = new DecoratedName(this);
            return !this.UnDecorator.DoMicrosoftKeywords
                ? result
                : this.Convention switch
                                    {
                                        ConventionType.C => result.Assign(this.UScore(TokenType.C)),
                                        ConventionType.Pascal => result.Assign(this.UScore(TokenType.Pascal)),
                                        ConventionType.This => result.Assign(this.UScore(TokenType.This)),
                                        ConventionType.Standard => result.Assign(this.UScore(TokenType.Standard)),
                                        ConventionType.Fast => result.Assign(this.UScore(TokenType.Fast)),
                                        ConventionType.CommonLanguageRuntime => result.Assign(this.UScore(TokenType.CommonLanguageRuntime)),
                                        _ => throw new System.InvalidOperationException()
                                    };
        }

        private unsafe void Parse(ref char* pSource)
        {
            if (*pSource == '\0')
            {
                this.IsTruncated = true;
                return;
            }

            char c;
            var range = (c = *pSource++) - 0x41;
            if (range > 12)
            {
                this.IsInvalid = true;
                return;
            }

            switch (c)
            {
                case 'A':
                case 'B':
                    this.Convention = ConventionType.C;
                    break;
                case 'C':
                case 'D':
                    this.Convention = ConventionType.Pascal;
                    break;
                case 'E':
                case 'F':
                    this.Convention = ConventionType.This;
                    break;
                case 'G':
                case 'H':
                    this.Convention = ConventionType.Standard;
                    break;
                case 'I':
                case 'J':
                    this.Convention = ConventionType.Fast;
                    break;
                case 'M':
                    this.Convention = ConventionType.CommonLanguageRuntime;
                    break;
            }
        }

        protected override DecoratedName GenerateCode()
        {
            char code;
            switch (this.Convention)
            {
                case ConventionType.C:
                case ConventionType.Pascal:
                case ConventionType.This:
                case ConventionType.Standard:
                case ConventionType.Fast:
                    code = (char)(((int)this.Convention << 1) + 'A');
                    break;
                case ConventionType.CommonLanguageRuntime:
                    code = 'M';
                    break;
                default:
                    throw new System.InvalidOperationException();
            }
            return new(this, code);
        }
    }
}