namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class ThrowTypes : ComplexElement
    {
        private ArgumentTypes argumentTypes;
        private bool isHidden;

        public ThrowTypes(ComplexElement parent)
            : base(parent)
        { }

        public unsafe ThrowTypes(ComplexElement parent, ref char* pSource)
            : base(parent)
        {
            this.Parse(ref pSource);
        }

        [Setting]
        public bool IsMissing { get; set; }

        [Setting]
        public bool? IsHidden
        {
            get { return this.IsMissing ? (bool?)null : this.isHidden; }
            set { if (value != null) this.isHidden = (bool)value; }
        }

        [Child]
        public ArgumentTypes ArgumentTypes
        {
            get { return this.IsMissing || this.isHidden ? null : this.argumentTypes; }
        }

        protected override void CreateEmptyElements()
        {
            if (this.argumentTypes == null) this.argumentTypes = new ArgumentTypes(this);
        }

        protected override DecoratedName GenerateName()
        {
            var name = new DecoratedName(this);
            if (!this.IsMissing)
            {
                if (this.isHidden) 
                    return name;
                name += " throw(";
                name += this.argumentTypes.Name;
                name += ')';
            }
            else
                name = name.Assign(" throw(") + NodeStatus.Truncated + ')';
            return name;
        }

        private unsafe void Parse(ref char* pSource)
        {                 
#pragma warning disable 665
            if (this.IsMissing = *pSource == '\0') 
                return;
            if (this.isHidden = *pSource == 'Z')
#pragma warning restore 665
                pSource++;
            else
                this.argumentTypes = new ArgumentTypes(this, ref pSource);
        }

        protected override DecoratedName GenerateCode()
        {
            var code = new DecoratedName(this);
            if (!this.IsMissing)
                if (this.isHidden)
                    code.Assign('Z');
                else
                    code.Assign(this.argumentTypes.Code);
            else
                code.Assign('\0');
            return code;
        }
    }
}