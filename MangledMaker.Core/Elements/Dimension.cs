namespace MangledMaker.Core.Elements
{
    using System.Text;
    using Attributes;

    public sealed class Dimension : Element
    {
        private readonly bool signed;

        private bool isNonTypeTemplateParameter;
        private long value;

        public Dimension(Element parent) : base(parent)
        { }

        public Dimension(Element parent, long value) : this(parent, value, value < 0)
        { }

        public Dimension(Element parent, long value, bool signed)
            : base(parent)
        {
            this.value = value;
            this.signed = signed;
        }

        public unsafe Dimension(Element parent, ref char* pSource, bool signed)
            : base(parent)
        {
            this.signed = signed;
            this.Parse(ref pSource);
        }

        [Setting]
        public long Value
        {
            get => this.value;
            set
            {
                this.value = value;
                if (value < 1 || value > 9) this.isNonTypeTemplateParameter = false;
            }
        }

        [Setting]
        public bool? IsNonTypeTemplateParameter
        {
            get => (this.value is > 0 and < 10) ? this.isNonTypeTemplateParameter : null;
            set => this.isNonTypeTemplateParameter = value ?? false;
        }

        protected override DecoratedName GenerateName() =>
            this.value switch
            {
                < 0 => new(this, this.value),
                < 10 when this.isNonTypeTemplateParameter => "`non-type-template-parameter-" +
                                                             new DecoratedName(this, (ulong) this.value),
                _ => new(this, (ulong) this.value)
            };

        private unsafe void Parse(ref char* pSource)
        {
            this.isNonTypeTemplateParameter = false;
            var cur = *pSource;
            if (cur == 'Q')
            {
                this.isNonTypeTemplateParameter = true;
                cur = *++pSource;
            }
            switch (cur)
            {
                case '\0':
                    this.IsTruncated = true;
                    break;
                case >= '0' and <= '9':
                    this.value = cur - 0x2F;
                    pSource++;
                    break;
                default:
                {
                    ulong rawValue = 0;
                    for (; cur != '@'; cur = *++pSource)
                        switch (cur)
                        {
                            case '\0':
                                this.IsTruncated = true;
                                return;
                            case < 'A':
                            case > 'P':
                                this.IsInvalid = true;
                                return;
                            default:
                                rawValue <<= 4;
                                rawValue += (ulong)(cur - 'A');
                                break;
                        }

                    pSource++;
                    //if (cur != '@')  // impossible
                    //    this.IsInvalid = true;
                    //else
                    this.value = this.signed ? *(long*)&rawValue : (long)rawValue;
                    break;
                }
            }
        }

        protected override DecoratedName GenerateCode()
        {
            StringBuilder result = new();

            if (this.isNonTypeTemplateParameter)
                result.Append('Q');

            if (this.value is > 0 and < 10)
                result.Append((char)(this.value + '\x2F'));
            else
            {
                ulong rawValue;
                unsafe
                {
                    var l = this.value;
                    rawValue = *(ulong*)&l;
                }

                var first = true;

                for (var i = 15; i >= 0; i--)
                {
                    var c = (char)((rawValue >> (i << 2) & 0xF) + 'A');
                    if (first)
                        first = c == 'A';
                    if (!first)
                        result.Append(c);
                }

                result.Append('@');
            }

            return new(this, result.ToString());
        }
    }
}