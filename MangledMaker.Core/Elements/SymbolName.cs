namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class SymbolName : ComplexElement
    {
        private bool isOperator;
        private bool isTemplate;
        private OperatorName operatorName;
        private TemplateName templateName;
        private ZName zName;

        public SymbolName(ComplexElement parent) : base(parent)
        { }

        public unsafe SymbolName(ComplexElement parent, ref char* pSource)
            : base(parent)
        {
            this.Parse(ref pSource);
        }

        [Setting]
        public bool? IsTemplate
        {
            get { return !this.isOperator && this.isTemplate; }
            set
            {
                if (value == null) 
                    return;
                this.isTemplate = (bool)value;
                if ((bool)value) 
                    this.isOperator = false;
            }
        }

        [Setting]
        public bool? IsOperator
        {
            get { return !this.isTemplate ? this.isOperator : (bool?)null; }
            set
            {
                if (value == null) 
                    return;
                this.isOperator = (bool)value;
                if ((bool)value) 
                    this.isTemplate = false;
            }
        }

        [Child]
        public TemplateName TemplateName
        {
            get { return this.isTemplate ? this.templateName : null; }
        }

        [Child]
        public OperatorName OperatorName
        {
            get { return this.isOperator ? this.operatorName : null; }
        }

        [Child]
        public ZName ZName
        {
            get { return this.isTemplate || this.isOperator ? null : this.zName; }
        }

        protected override void CreateEmptyElements()
        {
            if (this.templateName == null) 
                this.templateName = new TemplateName(this, true);
            if (this.operatorName == null) 
                this.operatorName = new OperatorName(this, false);
            if (this.zName == null) 
                this.zName = new ZName(this, true);
        }

        protected override DecoratedName GenerateName()
        {
            if (this.isTemplate)
                return this.templateName.Name;
            return this.isOperator ? this.operatorName.Name : this.zName.Name;
        }

        private unsafe void Parse(ref char* pSource)
        {
            if (*pSource == '?')
                if (pSource[1] == '$')
                {
                    this.templateName = new TemplateName(this, ref pSource, true);
                    this.isTemplate = true;
                }
                else
                {
                    pSource++;
                    this.operatorName = new OperatorName(this, ref pSource, false);
                    this.isOperator = true;
                }
            else
                this.zName = new ZName(this, ref pSource, true);
        }

        protected override DecoratedName GenerateCode()
        {
            if (this.isTemplate)
                return this.templateName.Code;

            if (this.isOperator)
                return new DecoratedName(this, '?') + this.operatorName.Code;

            return this.zName.Code;
        }
    }
}