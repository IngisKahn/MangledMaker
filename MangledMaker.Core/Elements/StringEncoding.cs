namespace MangledMaker.Core.Elements
{
    using System.Text;
    using Attributes;

    public sealed class StringEncoding : Element
    {
        public StringEncoding(Element parent, string prefix, string value) : base(parent)
        {
            this.Prefix = prefix;
            this.Value = value;
            this.Length = new(this, 0);
            this.Checksum = new(this, 0);
        }

        public unsafe StringEncoding(Element parent, ref char* pSource, string prefix) : base(parent)
        {
            this.Prefix = prefix;
            this.Parse(ref pSource);
            this.Value ??= string.Empty;
            this.Length ??= new(this);
            this.Checksum ??= new(this);
        }

        [Input]
        public string Prefix { get; set; }

        [Child]
        public Dimension Length { get; private set; }

        [Child]
        public Dimension Checksum { get; private set; }

        [Setting]
        public string Value { get; set; }

        protected override DecoratedName GenerateName() => new(this, this.Prefix);

        private unsafe void Parse(ref char* pSource)
        {
            if (*pSource++ == '@')
            {
                if (*pSource++ != '_')
                {
                    this.IsInvalid = true;
                    return;
                }
                pSource++; // text width 0=8-bit 1=16
                this.Length = new(this, ref pSource, false);
                this.Checksum = new(this, ref pSource, false);

                StringBuilder stringValue = new();

                while (*pSource != '\0' && *pSource != '@')
                    stringValue.Append(*pSource++);

                this.Value = stringValue.ToString();

                if (*pSource == '\0')
                {
                    pSource--;
                    this.IsTruncated = true;
                    return;
                }
                pSource++;
            }
            else
                this.IsInvalid = true;
        }

        protected override DecoratedName GenerateCode() => "@_0" + new DecoratedName(this, this.Length.Code + this.Checksum.Code) + this.Value + '@';
    }
}