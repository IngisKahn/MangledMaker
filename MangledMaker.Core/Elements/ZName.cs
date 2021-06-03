namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class ZName : ComplexElement
    {
        #region ZNameType enum

        public enum ZNameType
        {
            Index,
            Template,
            TemplateParameter,
            GenericType,
            Name
        }

        #endregion

        private const string template = "template-parameter-", generic = "generic-type-";

        private CustomName genericType;
        private TerminatedName simpleName;
        private TemplateName templateName;
        private CustomName templateParameter;

        private int zIndex;

        public ZName(ComplexElement parent, bool updateChachedNames)
            : base(parent)
        {
            this.UpdateCachedNames = updateChachedNames;
        }

        public unsafe ZName(ComplexElement parent, ref char* pSource, bool updateChachedNames)
            : this(parent, updateChachedNames)
        {
            this.Parse(ref pSource);
        }

        [Input]
        public bool UpdateCachedNames { get; private set; }

        [Setting]
        public ZNameType Type { get; set; }

        [Setting(RangeFrom = 0, RangeTo = 9)]
        public int? ZIndex
        {
            get { return this.Type == ZNameType.Index ? this.zIndex : (int?)null; }
            set { if (value != null) this.zIndex = (int)value; }
        }

        [Child]
        public TemplateName TemplateName
        {
            get { return this.Type == ZNameType.Template ? this.templateName : null; }
            set { this.templateName = value; }
        }

        [Child]
        public CustomName TemplateParameter
        {
            get { return this.Type == ZNameType.TemplateParameter ? this.templateParameter : null; }
            set { this.templateParameter = value; }
        }

        [Child]
        public CustomName GenericType
        {
            get { return this.Type == ZNameType.GenericType ? this.genericType : null; }
            set { this.genericType = value; }
        }

        [Child]
        public TerminatedName SimpleName
        {
            get { return this.Type == ZNameType.Name ? this.simpleName : null; }
            set { this.simpleName = value; }
        }

        protected override void CreateEmptyElements()
        {
            if (this.templateName == null) 
                this.templateName = new TemplateName(this, false);
            if (this.templateParameter == null)
                this.templateParameter = new CustomName(this, template);
            if (this.genericType == null) 
                this.genericType = new CustomName(this, generic);
            if (this.simpleName == null) 
                this.simpleName = new TerminatedName(this, '@');
        }

        protected override DecoratedName GenerateName()
        {
            DecoratedName result;
            switch (this.Type)
            {
                case ZNameType.Index:
                    return this.UnDecorator.ZNameList[this.zIndex];
                case ZNameType.Template:
                    result = this.templateName.Name;
                    break;
                case ZNameType.TemplateParameter:
                    result = this.templateParameter.Name;
                    break;
                case ZNameType.GenericType:
                    result = this.genericType.Name;
                    break;
                default:
                    result = this.simpleName.Name;
                    break;
            }

            // add to ZNameList if we're able and willing
            //if (this.UpdateCachedNames && !this.UnDecorator.ZNameList.IsFull)
            //    this.UnDecorator.ZNameList.Append(result);

            return result;
        }

        private unsafe void Parse(ref char* pSource)
        {
            //	Handle 'zname-replicators', otherwise an actual name
            // if ZName is a digit then return the ZNameList item
            var zcurrentCharacter = *pSource - '0';
            if (zcurrentCharacter >= 0 && zcurrentCharacter <= 9)
            {
                pSource++;	// Skip past the replicator
                //	And return the indexed name
                this.Type = ZNameType.Index;
                this.zIndex = zcurrentCharacter;
                return;
            }

            if (*pSource == '?')
            {
                this.Type = ZNameType.Template;
                this.templateName = new TemplateName(this, ref pSource, false);
                if (*pSource++ != '@')
                    if (*--pSource == '\0')
                        this.IsInvalid = true;
                    else
                        this.IsTruncated = true;
            }
            else if (ConsumeString(template, ref pSource))
            {
                this.templateParameter = new CustomName(this, ref pSource, template);
                this.Type = ZNameType.TemplateParameter;
            }
            else if (ConsumeString(generic, ref pSource))
            {
                this.genericType = new CustomName(this, ref pSource, generic);
                this.Type = ZNameType.GenericType;
            }
            else
            {
                //	Extract the 'zname' to the terminator
                this.simpleName = new TerminatedName(this, ref pSource, '@');
                this.Type = ZNameType.Name;
            }


            // add to ZNameList if we're able and willing
            //var result = this.Name;
            if (this.UpdateCachedNames && !this.UnDecorator.ZNameList.IsFull)
                this.UnDecorator.ZNameList.Append(this);
        }

        private static unsafe bool ConsumeString(string target, ref char* pSource)
        {
            if (!new string(pSource).StartsWith(target)) 
                return false;
            pSource += target.Length;
            return true;
        }

        protected override DecoratedName GenerateCode()
        {
            switch (this.Type)
            {
                case ZNameType.Index:
                    return new DecoratedName(this, (char)(this.zIndex + '0'));
                case ZNameType.Template:
                    return this.templateName.Code;
                case ZNameType.TemplateParameter:
                    return new DecoratedName(this, template) + this.templateParameter.Code;
                case ZNameType.GenericType:
                    return new DecoratedName(this, generic) + this.genericType.Code;
                default:
                    return this.simpleName.Code;
            }
        }
    }
}