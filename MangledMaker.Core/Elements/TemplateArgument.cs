namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class TemplateArgument : ComplexElement
    {
        public enum TemplateArgumentTypes
        {
            Saved1,
            Saved2,
            Saved3,
            Saved4,
            Saved5,
            Saved6,
            Saved7,
            Saved8,
            Saved9,
            Saved10,
            Void,
            Constant,
            Parameter,
            Type
        }

        public TemplateArgument(ComplexElement parent)
            : base(parent)
        {
        }

        public unsafe TemplateArgument(ComplexElement parent, ref char* pSource)
            : base(parent) =>
            this.Parse(ref pSource);

        [Setting]
        public TemplateArgumentTypes ArgumentType { get; set; }

        private TemplateConstant? constant;
        [Child]
        public TemplateConstant Constant { get => this.constant ??= new(this); private set => this.constant = value; }

        private SignedDimension? parameterIndex;
        [Child]
        public SignedDimension ParameterIndex { get => this.parameterIndex ??= new(this, true) { FixedSign = true }; private set => this.parameterIndex = value; }

        private PrimaryDataType? type;
        [Child]
        public PrimaryDataType Type { get => this.type ??= new(this, new()); private set => this.type = value; }

        protected override DecoratedName GenerateName()
        {
            var result = new DecoratedName(this);
            switch (this.ArgumentType)
            {
                case TemplateArgumentTypes.Saved1:
                case TemplateArgumentTypes.Saved2:
                case TemplateArgumentTypes.Saved3:
                case TemplateArgumentTypes.Saved4:
                case TemplateArgumentTypes.Saved5:
                case TemplateArgumentTypes.Saved6:
                case TemplateArgumentTypes.Saved7:
                case TemplateArgumentTypes.Saved8:
                case TemplateArgumentTypes.Saved9:
                case TemplateArgumentTypes.Saved10:
                    return this.UnDecorator.TemplateArgList[(int)this.ArgumentType];
                case TemplateArgumentTypes.Void:
                    result.Assign("void");
                    break;
                case TemplateArgumentTypes.Constant:
                    result.Assign(this.Constant.Name);
                    break;
                case TemplateArgumentTypes.Parameter:
                    var index = this.ParameterIndex.Name;
                    if (this.UnDecorator.HaveTemplateParameters)
                    {
                        var p = this.UnDecorator.GetParameter(int.Parse(index.ToString()));
                        if (!string.IsNullOrEmpty(p))
                            result.Assign(p);
                        else
                        {
                            result.Assign("`template-parameter");
                            result.Append(index);
                            result.Append('\'');
                        }
                    }
                    else
                    {
                        result.Assign("`template-parameter");
                        result.Append(index);
                        result.Append('\'');
                    }
                    break;
                case TemplateArgumentTypes.Type:
                    result.Assign(this.Type.Name);
                    break;
            }

            //if (result.Length > 1 && !this.UnDecorator.TemplateArgList.IsFull)
            //    this.UnDecorator.TemplateArgList.Append(result);
            return result;
        }

        private unsafe void Parse(ref char* pSource)
        {
            var numeric = *pSource - 30;
            if (numeric is >= 0 and <= 9)
            {
                pSource++;
                this.ArgumentType = (TemplateArgumentTypes)numeric;
            }
            else
            {
                switch (*pSource)
                {
                    case 'X':
                        pSource++;
                        this.ArgumentType = TemplateArgumentTypes.Void;
                        break;
                    case '$' when pSource[1] != '$':
                        pSource++;
                        this.Constant = new(this, ref pSource);
                        this.ArgumentType = TemplateArgumentTypes.Constant;
                        break;
                    case '?':
                        this.ParameterIndex = new(this, ref pSource) { FixedSign = true };
                        this.ArgumentType = TemplateArgumentTypes.Parameter;
                        break;
                    default:
                        this.Type = new(this, ref pSource, new());
                        this.ArgumentType = TemplateArgumentTypes.Type;
                        break;
                }

                var result = this.Name;
                if (result.Length > 1 && !this.UnDecorator.TemplateArgList.IsFull)
                    this.UnDecorator.TemplateArgList.Append(this);
            }
        }

        protected override DecoratedName GenerateCode()
        {
            DecoratedName result = new(this);
            switch (this.ArgumentType)
            {
                case TemplateArgumentTypes.Saved1:
                case TemplateArgumentTypes.Saved2:
                case TemplateArgumentTypes.Saved3:
                case TemplateArgumentTypes.Saved4:
                case TemplateArgumentTypes.Saved5:
                case TemplateArgumentTypes.Saved6:
                case TemplateArgumentTypes.Saved7:
                case TemplateArgumentTypes.Saved8:
                case TemplateArgumentTypes.Saved9:
                case TemplateArgumentTypes.Saved10:
                    result.Assign((char)((int)this.ArgumentType + '0'));
                    break;
                case TemplateArgumentTypes.Void:
                    result.Assign('X');
                    break;
                case TemplateArgumentTypes.Constant:
                    result.Assign('$');
                    result.Append(this.Constant.Code);
                    break;
                case TemplateArgumentTypes.Parameter:
                    result.Assign(this.ParameterIndex.Code);
                    break;
                case TemplateArgumentTypes.Type:
                    result.Assign(this.Type.Code);
                    break;
            }
            return result;
        }
    }
}