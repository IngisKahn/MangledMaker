namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class Main : ComplexElement
    {
        private FullName fullName;

        private bool isTemplate;

        private string? name;
        private TemplateName templateName;

        public Main(UnDecorator unDecorator) : base(unDecorator)
        {
            this.fullName = new FullName(this);
            this.templateName = new TemplateName(this, false);
        }

        public unsafe Main(ref char* pSource, UnDecorator unDecorator) : this(unDecorator) => this.Parse(ref pSource);

        [Child]
        public FullName? FullName => this.IsCodeView || !this.isTemplate ? this.fullName : null;

        [Child]
        public TemplateName? TemplateName => !this.IsCodeView && this.isTemplate ? this.templateName : null;

        [Setting]
        public bool IsCodeView { get; set; }

        [Setting]
        public bool? IsTemplate
        {
            get => this.IsCodeView ? null : this.isTemplate;
            set { if (value != null) this.isTemplate = (bool)value; }
        }

        protected override DecoratedName GenerateName()
        {
            //this.UnDecorator.Reset();

            DecoratedName result = this.IsCodeView
                ? "CV: " + new DecoratedName(this, this.fullName.Name)
                : new(this.isTemplate ? this.templateName.Name : this.fullName.Name);

            DecoratedName unDName = result.Status == NodeStatus.Invalid || this.UnDecorator.DoNameOnly && this.name?.Length != 0
                ? new(this, this.name)
                : result;

            return unDName;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.UnDecorator.Reset();

            this.isTemplate = this.IsCodeView = false;

            this.name = new string(pSource);

            switch (*pSource)
            {
                case '\0':
                    this.IsTruncated = true; // do we want this?
                    return;
                case '?' when pSource[1] == '@':
                    pSource += 2;
                    this.IsCodeView = true;
                    //templateName = new TemplateName(this, unDecorator);
                    this.fullName = new(this, ref pSource);
                    break;
                case '?' when pSource[1] == '$':
                {
                    var pOriginal = pSource;
                    this.templateName = new(this, ref pSource, false);
                    if (this.templateName.Name.Status == NodeStatus.Invalid)
                    {
                        pSource = pOriginal;
                        //templateName = new TemplateName(this, unDecorator);
                        this.fullName = new(this, ref pSource);
                    }
                    else
                        this.isTemplate = true;

                    break;
                }
                default:
                    //templateName = new TemplateName(this, unDecorator);
                    this.fullName = new(this, ref pSource);
                    break;
            }
        }

        protected override DecoratedName GenerateCode()
        {
            DecoratedName result = new(this);

            if (this.IsCodeView)
                result += "?@";
            result += this.isTemplate ? this.templateName.Code : this.fullName.Code;
            return result;
        }
    }
}