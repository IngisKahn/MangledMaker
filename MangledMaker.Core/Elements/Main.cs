namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class Main : ComplexElement
    {
        private FullName fullName;

        private bool isTemplate;

        private string name;
        private TemplateName templateName;

        public Main(UnDecorator unDecorator) : base(unDecorator)
        {
            this.fullName = new FullName(this);
            this.templateName = new TemplateName(this, false);
        }

        public unsafe Main(ref char* pSource, UnDecorator unDecorator) : this(unDecorator)
        {
            this.Parse(ref pSource);
        }

        [Child]
        public FullName FullName
        {
            get { return this.IsCodeView || !this.isTemplate ? this.fullName : null; }
        }

        [Child]
        public TemplateName TemplateName
        {
            get { return !this.IsCodeView && this.isTemplate ? this.templateName : null; }
        }

        [Setting]
        public bool IsCodeView { get; set; }

        [Setting]
        public bool? IsTemplate
        {
            get { return this.IsCodeView ? null : (bool?)this.isTemplate; }
            set { if (value != null) this.isTemplate = (bool)value; }
        }

        protected override DecoratedName GenerateName()
        {
            DecoratedName result;
            DecoratedName unDName;

            //this.UnDecorator.Reset();

            if (this.IsCodeView)
                result = "CV: " + new DecoratedName(this, this.fullName.Name);
            else
                result = new DecoratedName(this.isTemplate ? this.templateName.Name : this.fullName.Name);

            if (result.Status == NodeStatus.Invalid || this.UnDecorator.DoNameOnly && this.name.Length != 0)
                unDName = new DecoratedName(this, this.name);
            else
                unDName = result;

            return unDName;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.UnDecorator.Reset();

            this.isTemplate = this.IsCodeView = false;

            this.name = new string(pSource);

            if (*pSource == '\0')
            {
                this.IsTruncated = true; // do we want this?
                return;
            }

            if (*pSource == '?' && pSource[1] == '@')
            {
                pSource += 2;
                this.IsCodeView = true;
                //templateName = new TemplateName(this, unDecorator);
                this.fullName = new FullName(this, ref pSource);
            }
            else if (*pSource == '?' && pSource[1] == '$')
            {
                var pOriginal = pSource;
                this.templateName = new TemplateName(this, ref pSource, false);
                if (this.templateName.Name.Status == NodeStatus.Invalid)
                {
                    pSource = pOriginal;
                    //templateName = new TemplateName(this, unDecorator);
                    this.fullName = new FullName(this, ref pSource);
                }
                else
                    this.isTemplate = true;
            }
            else
            {
                //templateName = new TemplateName(this, unDecorator);
                this.fullName = new FullName(this, ref pSource);
            }
        }

        protected override DecoratedName GenerateCode()
        {
            var result = new DecoratedName(this);

            if (this.IsCodeView)
                result += "?@";
            result += this.isTemplate ? this.templateName.Code : this.fullName.Code;
            return result;
        }
    }
}