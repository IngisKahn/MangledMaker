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
            get { return this.value; }
            set
            {
                this.value = value;
                if (value < 1 || value > 9) this.isNonTypeTemplateParameter = false;
            }
        }

        [Setting]
        public bool? IsNonTypeTemplateParameter
        {
            get { return (this.value > 0 && this.value < 10) ? (bool?)this.isNonTypeTemplateParameter : null; }
            set { this.isNonTypeTemplateParameter = value ?? false; }
        }

        protected override DecoratedName GenerateName()
        {
            if (this.value < 0)
                return new DecoratedName(this, this.value);
            if (this.value < 10 && this.isNonTypeTemplateParameter)
                return "`non-type-template-parameter-" + new DecoratedName(this, (ulong)this.value);
            return new DecoratedName(this, (ulong)this.value);
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.isNonTypeTemplateParameter = false;
            var cur = *pSource;
            if (cur == 'Q')
            {
                this.isNonTypeTemplateParameter = true;
                cur = *++pSource;
            }
            if (cur == '\0')
                this.IsTruncated = true;
            else if (cur >= '0' && cur <= '9')
            {
                this.value = cur - 0x2F;
                pSource++;
            }
            else
            {
                ulong rawValue = 0;
                for (; cur != '@'; cur = *++pSource)
                {
                    if (cur == '\0')
                    {
                        this.IsTruncated = true;
                        return;
                    }
                    if (cur < 'A' || cur > 'P')
                    {
                        this.IsInvalid = true;
                        return;
                    }
                    rawValue <<= 4;
                    rawValue += (ulong)(cur - 'A');
                }
                pSource++;
                //if (cur != '@')  // impossible
                //    this.IsInvalid = true;
                //else
                this.value = this.signed ? *(long*)&rawValue : (long)rawValue;
            }
        }

        protected override DecoratedName GenerateCode()
        {
            var result = new StringBuilder();

            if (this.isNonTypeTemplateParameter)
                result.Append('Q');

            if (this.value > 0 && this.value < 10)
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

            return new DecoratedName(this, result.ToString());
        }
    }
}