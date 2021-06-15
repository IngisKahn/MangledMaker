namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class SymbolName : ComplexElement
    {
        private bool isOperator;
        private bool isTemplate;
        private OperatorName? operatorName;
        private OperatorName OperatorNameSafe => this.operatorName ??= new(this, false);
        private TemplateName? templateName;
        private TemplateName TemplateNameSafe => this.templateName ??= new(this, true);
        private ZName? zName;
        private ZName ZNameSafe => this.zName ??= new(this, true);

        public SymbolName(ComplexElement parent) : base(parent)
        { }

        public unsafe SymbolName(ComplexElement parent, ref char* pSource)
            : base(parent) =>
            this.Parse(ref pSource);

        [Setting]
        public bool? IsTemplate
        {
            get => !this.isOperator && this.isTemplate;
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
            get => !this.isTemplate ? this.isOperator : null;
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
        public TemplateName? TemplateName => this.isTemplate ? this.templateName : null;

        [Child]
        public OperatorName? OperatorName => this.isOperator ? this.operatorName : null;

        [Child]
        public ZName? ZName => this.isTemplate || this.isOperator ? null : this.zName;

        protected override DecoratedName GenerateName() =>
            this.isTemplate ? this.TemplateNameSafe.Name :
            this.isOperator ? this.OperatorNameSafe.Name : this.ZNameSafe.Name;

        private unsafe void Parse(ref char* pSource)
        {
            if (*pSource == '?')
                if (pSource[1] == '$')
                {
                    this.templateName = new(this, ref pSource, true);
                    this.isTemplate = true;
                }
                else
                {
                    pSource++;
                    this.operatorName = new(this, ref pSource, false);
                    this.isOperator = true;
                }
            else
                this.zName = new(this, ref pSource, true);
        }

        protected override DecoratedName GenerateCode()
        {
            if (this.isTemplate)
                return this.TemplateNameSafe.Code;

            if (this.isOperator)
                return new DecoratedName(this, '?') + this.OperatorNameSafe.Code;

            return this.ZNameSafe.Code;
        }
    }
}