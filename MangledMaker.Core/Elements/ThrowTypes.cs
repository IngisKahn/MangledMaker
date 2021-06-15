namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class ThrowTypes : ComplexElement
    {
        private ArgumentTypes? argumentTypes;
        private ArgumentTypes ArgumentTypesSafe => this.argumentTypes ??= new(this);
        private bool isHidden;

        public ThrowTypes(ComplexElement parent)
            : base(parent)
        { }

        public unsafe ThrowTypes(ComplexElement parent, ref char* pSource)
            : base(parent) =>
            this.Parse(ref pSource);

        [Setting]
        public bool IsMissing { get; set; }

        [Setting]
        public bool? IsHidden
        {
            get => this.IsMissing ? null : this.isHidden;
            set { if (value != null) this.isHidden = (bool)value; }
        }

        [Child]
        public ArgumentTypes? ArgumentTypes => this.IsMissing || this.isHidden ? null : this.argumentTypes;

        protected override DecoratedName GenerateName()
        {
            DecoratedName name = new(this);
            if (!this.IsMissing)
            {
                if (this.isHidden) 
                    return name;
                name += " throw(";
                name += this.ArgumentTypesSafe.Name;
                name += ')';
            }
            else
                name = name.Assign(" throw(") + NodeStatus.Truncated + ')';
            return name;
        }

        private unsafe void Parse(ref char* pSource)
        {                 
            // ReSharper disable AssignmentInConditionalExpression
            if (this.IsMissing = *pSource == '\0') 
                return;
            if (this.isHidden = *pSource == 'Z')
                // ReSharper restore AssignmentInConditionalExpression
                pSource++;
            else
                this.argumentTypes = new(this, ref pSource);
        }

        protected override DecoratedName GenerateCode()
        {
            var code = new DecoratedName(this);
            if (!this.IsMissing)
                if (this.isHidden)
                    code.Assign('Z');
                else
                    code.Assign(this.ArgumentTypesSafe.Code);
            else
                code.Assign('\0');
            return code;
        }
    }
}