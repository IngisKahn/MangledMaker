namespace MangledMaker.Core.Elements
{
    using System;
    using Attributes;

    public sealed class TemplateConstant : ComplexElement
    {
        public enum TemplateConstantType
        {
            Integer,
            Null,
            Reference,
            Float,
            Parameter,
            NonTypeParameter,
            FullName,
            IndexPair,
            IndexTriple,
            NamedIndex,
            NamedIndexPair,
            NamedIndexTriple,
            ZName
        }

        private SignedDimension exponent;
        private FullName fullName;
        private SignedDimension index1;
        private SignedDimension index2;
        private SignedDimension index3;
        private SignedDimension integer;
        private SignedDimension mantissa;
        private SignedDimension parameterIndex;
        private ZName zName;

        public TemplateConstant(ComplexElement parent)
            : base(parent)
        {
        }

        public unsafe TemplateConstant(ComplexElement parent, ref char* pSource)
            : base(parent)
        {
            this.Parse(ref pSource);
        }

        [Setting]
        public TemplateConstantType TypeCategory { get; set; }

        [Child]
        public SignedDimension Integer
        {
            get { return this.TypeCategory == TemplateConstantType.Integer ? this.integer : null; }
        }

        [Child]
        public FullName FullName
        {
            get
            {
                switch (this.TypeCategory)
                {
                    case TemplateConstantType.Reference:
                    case TemplateConstantType.FullName:
                    case TemplateConstantType.NamedIndex:
                    case TemplateConstantType.NamedIndexPair:
                    case TemplateConstantType.NamedIndexTriple:
                        return this.fullName;
                }
                return null;
            }
        }

        [Child]
        public SignedDimension Mantissa
        {
            get { return this.TypeCategory == TemplateConstantType.Float ? this.mantissa : null; }
        }

        [Child]
        public SignedDimension Exponent
        {
            get { return this.TypeCategory == TemplateConstantType.Float ? this.exponent : null; }
        }


        [Child]
        public SignedDimension ParameterIndex
        {
            get
            {
                return this.TypeCategory == TemplateConstantType.Parameter
                       || this.TypeCategory == TemplateConstantType.NonTypeParameter
                           ? this.parameterIndex
                           : null;
            }
        }

        [Child]
        public SignedDimension Index1
        {
            get
            {
                switch (this.TypeCategory)
                {
                    case TemplateConstantType.IndexPair:
                    case TemplateConstantType.IndexTriple:
                    case TemplateConstantType.NamedIndex:
                    case TemplateConstantType.NamedIndexPair:
                    case TemplateConstantType.NamedIndexTriple:
                    case TemplateConstantType.ZName: // this is not output
                        return this.index1;
                }
                return null;
            }
        }

        [Child]
        public SignedDimension Index2
        {
            get
            {
                switch (this.TypeCategory)
                {
                    case TemplateConstantType.IndexPair:
                    case TemplateConstantType.IndexTriple:
                    case TemplateConstantType.NamedIndexPair:
                    case TemplateConstantType.NamedIndexTriple:
                        return this.index2;
                }
                return null;
            }
        }

        [Child]
        public SignedDimension Index3
        {
            get
            {
                switch (this.TypeCategory)
                {
                    case TemplateConstantType.IndexTriple:
                    case TemplateConstantType.NamedIndexTriple:
                        return this.index3;
                }
                return null;
            }
        }

        [Child]
        public ZName ZName
        {
            get { return this.TypeCategory == TemplateConstantType.ZName ? this.zName : null; }
        }

        protected override void CreateEmptyElements()
        {
            if (this.integer == null) this.integer = new SignedDimension(this, false);
            if (this.fullName == null) this.fullName = new FullName(this);
            if (this.mantissa == null) this.mantissa = new SignedDimension(this, false);
            if (this.exponent == null) this.exponent = new SignedDimension(this, false);
            if (this.parameterIndex == null) this.parameterIndex = new SignedDimension(this, false);
            if (this.index1 == null) this.index1 = new SignedDimension(this, false);
            if (this.index2 == null) this.index2 = new SignedDimension(this, false);
            if (this.index3 == null) this.index3 = new SignedDimension(this, false);
            if (this.zName == null) this.zName = new ZName(this, false);
        }

        protected override DecoratedName GenerateName()
        {
            DecoratedName result;
            switch (this.TypeCategory)
            {
                case TemplateConstantType.Integer:
                    return this.integer.Name;
                case TemplateConstantType.Null:
                    return new DecoratedName(this, "NULL");
                case TemplateConstantType.Reference:
                    return new DecoratedName(this, '&') + this.fullName.Name;
                case TemplateConstantType.Float:
                    var mant = this.mantissa.Name.ToString();
                    result = new DecoratedName(this);
                    if (mant[0] == '-')
                    {
                        result.Append(new DecoratedName(this.mantissa, mant.Substring(0, 2)));
                        result.Append('.');
                        result.Append(new DecoratedName(this.mantissa, mant.Substring(2)));
                    }
                    else
                    {
                        result.Append(new DecoratedName(this.mantissa, mant[0]));
                        result.Append('.');
                        result.Append(new DecoratedName(this.mantissa, mant.Substring(1)));
                    }
                    result.Append('e');
                    return result + this.exponent.Name;
                case TemplateConstantType.Parameter:
                case TemplateConstantType.NonTypeParameter:
                    var index = this.parameterIndex.Name;
                    if (this.UnDecorator.HaveTemplateParameters)
                    {
                        var param = this.UnDecorator.GetParameter(int.Parse(index.ToString()));
                        if (!string.IsNullOrEmpty(param))
                            return new DecoratedName(this, param);
                    }
                    result = new DecoratedName(this,
                                               this.TypeCategory == TemplateConstantType.Parameter
                                                   ? "`template-parameter"
                                                   : "`non-type-template-parameter");
                    result.Append(index);
                    return result + '\'';
                case TemplateConstantType.FullName:
                    return this.fullName.Name;
                case TemplateConstantType.IndexPair:
                case TemplateConstantType.IndexTriple:
                case TemplateConstantType.NamedIndex:
                case TemplateConstantType.NamedIndexPair:
                case TemplateConstantType.NamedIndexTriple:
                    result = new DecoratedName('{');
                    if (this.TypeCategory >= TemplateConstantType.NamedIndex)
                    {
                        result.Append(this.fullName.Name);
                        result.Append(',');
                    }
                    switch (this.TypeCategory)
                    {
                        case TemplateConstantType.IndexPair:
                        case TemplateConstantType.NamedIndexPair:
                            result.Append(this.index1.Name);
                            result.Append(',');
                            result.Append(this.index2.Name);
                            break;
                        case TemplateConstantType.IndexTriple:
                        case TemplateConstantType.NamedIndexTriple:
                            result.Append(this.index1.Name);
                            result.Append(',');
                            result.Append(this.index2.Name);
                            result.Append(',');
                            result.Append(this.index3.Name);
                            break;
                        case TemplateConstantType.NamedIndex:
                            result.Append(this.index1.Name);
                            break;
                    }
                    return result + '}';
                case TemplateConstantType.ZName:
                    return this.zName.Name;
                default:
                    throw new InvalidOperationException();
            }
        }

        private unsafe void Parse(ref char* pSource)
        {
            var tempTypeCategory = *pSource++;
            switch (tempTypeCategory)
            {
                case '\0':
                    pSource--;
                    this.IsTruncated = true;
                    break;
                case '0':
                    this.TypeCategory = TemplateConstantType.Integer;
                    this.integer = new SignedDimension(this, ref pSource);
                    break;
                case '1':
                    if (*pSource == '@')
                    {
                        pSource++;
                        this.TypeCategory = TemplateConstantType.Null;
                    }
                    else
                    {
                        this.TypeCategory = TemplateConstantType.Reference;
                        this.fullName = new FullName(this, ref pSource);
                    }
                    break;
                case '2':
                    this.TypeCategory = TemplateConstantType.Float;
                    this.mantissa = new SignedDimension(this, ref pSource);
                    this.exponent = new SignedDimension(this, ref pSource);
                    var mn = this.mantissa.Name;
                    var en = this.exponent.Name;
                    if (mn.IsValid && en.IsValid)
                    {
                        var s = mn.ToString();
                        if (string.IsNullOrEmpty(s))
                            this.IsInvalid = true;
                        else
                            this.TypeCategory = TemplateConstantType.Float;
                    }
                    else
                        this.IsTruncated = true;
                    break;
                case 'D':
                case 'Q':
                    this.parameterIndex = new SignedDimension(this, ref pSource);
                    this.TypeCategory = tempTypeCategory == 'D' ? TemplateConstantType.Parameter : TemplateConstantType.NonTypeParameter;
                    break;
                case 'E':
                    this.fullName = new FullName(this, ref pSource);
                    break;
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                    if (tempTypeCategory >= 'H')
                        this.fullName = new FullName(this, ref pSource);
                    switch (tempTypeCategory)
                    {
                        case 'F':
                        case 'I':
                            this.index1 = new SignedDimension(this, ref pSource);
                            this.index2 = new SignedDimension(this, ref pSource);
                            break;
                        case 'G':
                        case 'J':
                            this.index1 = new SignedDimension(this, ref pSource);
                            this.index2 = new SignedDimension(this, ref pSource);
                            this.index3 = new SignedDimension(this, ref pSource);
                            break;
                        case 'H':
                            this.index1 = new SignedDimension(this, ref pSource);
                            break;
                    }
                    break;
                case 'R':
                    this.zName = new ZName(this, ref pSource, false);
                    this.index1 = new SignedDimension(this, ref pSource);
                    break;
                default:
                    this.IsInvalid = true;
                    break;
            }
        }

        protected override DecoratedName GenerateCode()
        {
            var result = new DecoratedName(this);
            var code = '\0';
            switch (this.TypeCategory)
            {
                case TemplateConstantType.Integer:
                    code = '0';
                    result.Assign(this.integer.Code);
                    break;
                case TemplateConstantType.Null:
                    code = '1';
                    result.Append('@');
                    break;
                case TemplateConstantType.Reference:
                    code = '1';
                    result.Assign(this.fullName.Code);
                    break;
                case TemplateConstantType.Float:
                    code = '2';
                    result.Assign(this.mantissa.Code + this.exponent.Code);
                    break;
                case TemplateConstantType.Parameter:
                    code = 'D';
                    result.Assign(this.parameterIndex.Code);
                    break;
                case TemplateConstantType.NonTypeParameter:
                    code = 'Q';
                    result.Assign(this.parameterIndex.Code);
                    break;
                case TemplateConstantType.FullName:
                    code = 'E';
                    result.Assign(this.fullName.Code);
                    break;
                case TemplateConstantType.IndexPair:
                case TemplateConstantType.IndexTriple:
                case TemplateConstantType.NamedIndex:
                case TemplateConstantType.NamedIndexPair:
                case TemplateConstantType.NamedIndexTriple:
                    if (this.TypeCategory >= TemplateConstantType.NamedIndex)
                        result.Assign(this.fullName.Code);
                    switch (this.TypeCategory)
                    {
                        case TemplateConstantType.IndexPair:
                        case TemplateConstantType.NamedIndexPair:
                            result.Append(this.index1.Code);
                            result.Append(this.index2.Code);
                            break;
                        case TemplateConstantType.IndexTriple:
                        case TemplateConstantType.NamedIndexTriple:
                            result.Append(this.index1.Code);
                            result.Append(this.index2.Code);
                            result.Append(this.index3.Code);
                            break;
                        case TemplateConstantType.NamedIndex:
                            result.Append(this.index1.Code);
                            break;
                    }
                    switch (this.TypeCategory)
                    {
                        case TemplateConstantType.IndexPair:
                            code = 'F';
                            break;
                        case TemplateConstantType.IndexTriple:
                            code = 'G';
                            break;
                        case TemplateConstantType.NamedIndex:
                            code = 'H';
                            break;
                        case TemplateConstantType.NamedIndexPair:
                            code = 'I';
                            break;
                        case TemplateConstantType.NamedIndexTriple:
                            code = 'J';
                            break;
                    }
                    break;
                case TemplateConstantType.ZName:
                    code = 'R';
                    result.Assign(this.zName.Code);
                    result.Append(this.index1.Code);
                    break;
            }
            return code + result;
        }
    }
}