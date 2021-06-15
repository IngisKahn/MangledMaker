namespace MangledMaker.Core.Elements
{
    using System.Diagnostics.CodeAnalysis;
    using Attributes;

    public sealed class ZName : ComplexElement
    {
        public enum ZNameType
        {
            Index,
            Template,
            TemplateParameter,
            GenericType,
            Name
        }

        private const string template = "template-parameter-", generic = "generic-type-";

        private CustomName? genericType;
        private TerminatedName? simpleName;
        private TemplateName? templateName;
        private CustomName? templateParameter;

        private int zIndex;

        public ZName(ComplexElement parent, bool updateCachedNames)
            : base(parent) =>
            this.UpdateCachedNames = updateCachedNames;

        public unsafe ZName(ComplexElement parent, ref char* pSource, bool updateCachedNames)
            : this(parent, updateCachedNames) =>
            this.Parse(ref pSource);

        [Input]
        public bool UpdateCachedNames { get; private set; }

        [Setting]
        public ZNameType Type { get; set; }

        [Setting(RangeFrom = 0, RangeTo = 9)]
        public int? ZIndex
        {
            get => this.Type == ZNameType.Index ? this.zIndex : (int?)null;
            set { if (value != null) this.zIndex = (int)value; }
        }

        [Child]
        public TemplateName? TemplateName => this.Type == ZNameType.Template ? this.templateName ??= new(this, false) : null;

        [Child]
        public CustomName? TemplateParameter => this.Type == ZNameType.TemplateParameter ? this.templateParameter ??= new(this, ZName.template) : null;

        [Child]
        public CustomName? GenericType => this.Type == ZNameType.GenericType ? this.genericType ??= new(this, ZName.generic) : null;

        [Child]
        public TerminatedName? SimpleName => this.Type == ZNameType.Name ? this.simpleName ??= new(this, '@') : null;

        protected override DecoratedName GenerateName()
        {
            DecoratedName result;
            switch (this.Type)
            {
                case ZNameType.Index:
                    return this.UnDecorator.ZNameList[this.zIndex];
                case ZNameType.Template:
                    result = this.TemplateName!.Name;
                    break;
                case ZNameType.TemplateParameter:
                    result = this.TemplateParameter!.Name;
                    break;
                case ZNameType.GenericType:
                    result = this.GenericType!.Name;
                    break;
                default:
                    result = this.SimpleName!.Name;
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
            if (zcurrentCharacter is >= 0 and <= 9)
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
                this.templateName = new(this, ref pSource, false);
                if (*pSource++ != '@')
                    if (*--pSource == '\0')
                        this.IsInvalid = true;
                    else
                        this.IsTruncated = true;
            }
            else if (ZName.ConsumeString(ZName.template, ref pSource))
            {
                this.templateParameter = new(this, ref pSource, ZName.template);
                this.Type = ZNameType.TemplateParameter;
            }
            else if (ZName.ConsumeString(ZName.generic, ref pSource))
            {
                this.genericType = new(this, ref pSource, ZName.generic);
                this.Type = ZNameType.GenericType;
            }
            else
            {
                //	Extract the 'zname' to the terminator
                this.simpleName = new(this, ref pSource, '@');
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

        protected override DecoratedName GenerateCode() =>
            this.Type switch
            {
                ZNameType.Index => new(this, (char) (this.zIndex + '0')),
                ZNameType.Template => this.TemplateName!.Code,
                ZNameType.TemplateParameter => new DecoratedName(this, ZName.template) + this.TemplateParameter!.Code,
                ZNameType.GenericType => new DecoratedName(this, ZName.generic) + this.GenericType!.Code,
                _ => this.SimpleName!.Code
            };
    }
}