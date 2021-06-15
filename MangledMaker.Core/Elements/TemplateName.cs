namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class TemplateName : ComplexElement
    {
        public TemplateName(ComplexElement parent, bool readTerminator) : base(parent) => this.ReadTerminator = readTerminator;

        public unsafe TemplateName(ComplexElement parent, ref char* pSource, bool readTerminator)
            : base(parent)
        {
            this.ReadTerminator = readTerminator;

            this.Parse(ref pSource);
        }

        [Input]
        public bool ReadTerminator { get; private set; }

        [Setting]
        public bool IsOperator { get; set; }

        private OperatorName? operatorName;
        [Child]
        public OperatorName OperatorName { get => this.operatorName ??= new(this, true); private set => this.operatorName = value; }

        private ZName? zName;
        [Child]
        public ZName ZName { get => this.zName ??= new(this, true); private set => this.zName = value; }

        private TemplateArgumentList? templateArgumentList;
        [Child]
        public TemplateArgumentList TemplateArgumentList { get => this.templateArgumentList ??= new(this); private set => this.templateArgumentList = value; }

        protected override DecoratedName GenerateName()
        {
            var fReadTemplateArguments = false;
            this.PushLists();

            DecoratedName templateName = new(this);

            if (this.IsOperator)
            {
                templateName.Assign(this.OperatorName.Name);
                fReadTemplateArguments = this.OperatorName.ReadTemplateArguments;
            }
            else
                templateName.Assign(this.ZName.Name);


            if (templateName.IsEmpty)
                this.UnDecorator.ExplicitTemplateParams = true;

            if (!fReadTemplateArguments)
            {
                templateName += '<';
                templateName += this.TemplateArgumentList.Name;
                if (templateName.LastCharacter == '>')
                    templateName += ' ';
                templateName += '>';
            }


            this.PopLists();
            return new(templateName);
        }

        private readonly Replicator localArgList = new();
        private readonly Replicator localZNameList = new();
        private readonly Replicator localTemplateArgList = new();
        private Replicator saveArgList;
        private Replicator saveZNameList;
        private Replicator saveTemplateArgList;

        private void PushLists()
        {
            this.saveArgList = this.UnDecorator.ArgList;
            this.saveZNameList = this.UnDecorator.ZNameList;
            this.saveTemplateArgList = this.UnDecorator.TemplateArgList;
            this.UnDecorator.ArgList = this.localArgList;
            this.UnDecorator.ZNameList = this.localZNameList;
            this.UnDecorator.TemplateArgList = this.localTemplateArgList;
        }

        private void PopLists()
        {
            this.UnDecorator.ArgList = this.saveArgList;
            this.UnDecorator.ZNameList = this.saveZNameList;
            this.UnDecorator.TemplateArgList = this.saveTemplateArgList;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.PushLists();
            if (*pSource != '?' || pSource[1] != '$')
                this.IsInvalid = true;
            else
            {
                pSource += 2;

                var fReadTemplateArguments = false;

                bool isEmpty;

                if (*pSource == '?')
                {
                    this.IsOperator = true;
                    pSource++;
                    this.OperatorName = new(this, ref pSource, true);
                    fReadTemplateArguments = this.OperatorName.ReadTemplateArguments;
                    isEmpty = this.OperatorName.Name.IsEmpty;
                }
                else
                {
                    this.ZName = new(this, ref pSource, true);
                    isEmpty = this.ZName.Name.IsEmpty;
                }

                if (isEmpty)
                    this.UnDecorator.ExplicitTemplateParams = true;

                if (!fReadTemplateArguments)
                {
                    this.TemplateArgumentList = new(this, ref pSource);

                    if (this.ReadTerminator)
                        pSource++;
                }
            }
            this.PopLists();
        }

        protected override DecoratedName GenerateCode()
        {
            DecoratedName result = new(this);

            var fReadTemplateArguments = false;
            if (this.IsOperator)
            {
                result += '?';
                result += this.OperatorName.Code;
                fReadTemplateArguments = this.OperatorName.ReadTemplateArguments;
            }
            else
                result += this.ZName.Code;

            if (fReadTemplateArguments) 
                return result;
            result += this.TemplateArgumentList.Code;
            if (this.ReadTerminator)
                result += '@';

            return result;
        }
    }
}