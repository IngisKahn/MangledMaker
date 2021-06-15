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

        private SignedDimension? exponent;
        private SignedDimension ExponentSafe => this.exponent ??= new(this, false);
        private FullName? fullName;
        private FullName FullNameSafe => this.fullName ??= new(this);
        private SignedDimension? index1;
        private SignedDimension Index1Safe => this.index1 ??= new(this, false);
        private SignedDimension? index2;
        private SignedDimension Index2Safe => this.index2 ??= new(this, false);
        private SignedDimension? index3;
        private SignedDimension Index3Safe => this.index3 ??= new(this, false);
        private SignedDimension? integer;
        private SignedDimension IntegerSafe => this.integer ??= new(this, false);
        private SignedDimension? mantissa;
        private SignedDimension MantissaSafe => this.mantissa ??= new(this, false);
        private SignedDimension? parameterIndex;
        private SignedDimension ParameterIndexSafe => this.parameterIndex ??= new(this, false);
        private ZName? zName;
        private ZName ZNameSafe => this.zName ??= new(this, false);

        public TemplateConstant(ComplexElement parent)
            : base(parent)
        {
        }

        public unsafe TemplateConstant(ComplexElement parent, ref char* pSource)
            : base(parent) =>
            this.Parse(ref pSource);

        [Setting]
        public TemplateConstantType TypeCategory { get; set; }

        [Child]
        public SignedDimension? Integer => this.TypeCategory == TemplateConstantType.Integer ? this.integer : null;

        [Child]
        public FullName? FullName
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
        public SignedDimension? Mantissa => this.TypeCategory == TemplateConstantType.Float ? this.mantissa : null;

        [Child]
        public SignedDimension? Exponent => this.TypeCategory == TemplateConstantType.Float ? this.exponent : null;


        [Child]
        public SignedDimension? ParameterIndex =>
            this.TypeCategory == TemplateConstantType.Parameter
            || this.TypeCategory == TemplateConstantType.NonTypeParameter
                ? this.parameterIndex
                : null;

        [Child]
        public SignedDimension? Index1
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
        public SignedDimension? Index2
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
        public SignedDimension? Index3
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
        public ZName? ZName => this.TypeCategory == TemplateConstantType.ZName ? this.zName : null;

        protected override DecoratedName GenerateName()
        {
            DecoratedName result;
            switch (this.TypeCategory)
            {
                case TemplateConstantType.Integer:
                    return this.IntegerSafe.Name;
                case TemplateConstantType.Null:
                    return new(this, "NULL");
                case TemplateConstantType.Reference:
                    return new DecoratedName(this, '&') + this.FullNameSafe.Name;
                case TemplateConstantType.Float:
                    var mant = this.MantissaSafe.Name.ToString();
                    result = new(this);
                    if (mant[0] == '-')
                    {
                        result.Append(new DecoratedName(this.mantissa, mant[..2]));
                        result.Append('.');
                        result.Append(new DecoratedName(this.mantissa, mant[2..]));
                    }
                    else
                    {
                        result.Append(new DecoratedName(this.mantissa, mant[0]));
                        result.Append('.');
                        result.Append(new DecoratedName(this.mantissa, mant[1..]));
                    }
                    result.Append('e');
                    return result + this.ExponentSafe.Name;
                case TemplateConstantType.Parameter:
                case TemplateConstantType.NonTypeParameter:
                    var index = this.ParameterIndexSafe.Name;
                    if (this.UnDecorator.HaveTemplateParameters)
                    {
                        var param = this.UnDecorator.GetParameter(int.Parse(index.ToString()));
                        if (!string.IsNullOrEmpty(param))
                            return new(this, param);
                    }
                    result = new(this,
                                               this.TypeCategory == TemplateConstantType.Parameter
                                                   ? "`template-parameter"
                                                   : "`non-type-template-parameter");
                    result.Append(index);
                    return result + '\'';
                case TemplateConstantType.FullName:
                    return this.FullNameSafe.Name;
                case TemplateConstantType.IndexPair:
                case TemplateConstantType.IndexTriple:
                case TemplateConstantType.NamedIndex:
                case TemplateConstantType.NamedIndexPair:
                case TemplateConstantType.NamedIndexTriple:
                    result = new('{');
                    if (this.TypeCategory >= TemplateConstantType.NamedIndex)
                    {
                        result.Append(this.FullNameSafe.Name);
                        result.Append(',');
                    }
                    switch (this.TypeCategory)
                    {
                        case TemplateConstantType.IndexPair:
                        case TemplateConstantType.NamedIndexPair:
                            result.Append(this.Index1Safe.Name);
                            result.Append(',');
                            result.Append(this.Index2Safe.Name);
                            break;
                        case TemplateConstantType.IndexTriple:
                        case TemplateConstantType.NamedIndexTriple:
                            result.Append(this.Index1Safe.Name);
                            result.Append(',');
                            result.Append(this.Index2Safe.Name);
                            result.Append(',');
                            result.Append(this.Index3Safe.Name);
                            break;
                        case TemplateConstantType.NamedIndex:
                            result.Append(this.Index1Safe.Name);
                            break;
                    }
                    return result + '}';
                case TemplateConstantType.ZName:
                    return this.ZNameSafe.Name;
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
                    this.integer = new(this, ref pSource);
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
                        this.fullName = new(this, ref pSource);
                    }
                    break;
                case '2':
                    this.TypeCategory = TemplateConstantType.Float;
                    this.mantissa = new(this, ref pSource);
                    this.exponent = new(this, ref pSource);
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
                    this.parameterIndex = new(this, ref pSource);
                    this.TypeCategory = tempTypeCategory == 'D' ? TemplateConstantType.Parameter : TemplateConstantType.NonTypeParameter;
                    break;
                case 'E':
                    this.fullName = new(this, ref pSource);
                    break;
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                    if (tempTypeCategory >= 'H')
                        this.fullName = new(this, ref pSource);
                    switch (tempTypeCategory)
                    {
                        case 'F':
                        case 'I':
                            this.index1 = new(this, ref pSource);
                            this.index2 = new(this, ref pSource);
                            break;
                        case 'G':
                        case 'J':
                            this.index1 = new(this, ref pSource);
                            this.index2 = new(this, ref pSource);
                            this.index3 = new(this, ref pSource);
                            break;
                        case 'H':
                            this.index1 = new(this, ref pSource);
                            break;
                    }
                    break;
                case 'R':
                    this.zName = new(this, ref pSource, false);
                    this.index1 = new(this, ref pSource);
                    break;
                default:
                    this.IsInvalid = true;
                    break;
            }
        }

        protected override DecoratedName GenerateCode()
        {
            DecoratedName result = new(this);
            var code = '\0';
            switch (this.TypeCategory)
            {
                case TemplateConstantType.Integer:
                    code = '0';
                    result.Assign(this.IntegerSafe.Code);
                    break;
                case TemplateConstantType.Null:
                    code = '1';
                    result.Append('@');
                    break;
                case TemplateConstantType.Reference:
                    code = '1';
                    result.Assign(this.FullNameSafe.Code);
                    break;
                case TemplateConstantType.Float:
                    code = '2';
                    result.Assign(this.MantissaSafe.Code + this.ExponentSafe.Code);
                    break;
                case TemplateConstantType.Parameter:
                    code = 'D';
                    result.Assign(this.ParameterIndexSafe.Code);
                    break;
                case TemplateConstantType.NonTypeParameter:
                    code = 'Q';
                    result.Assign(this.ParameterIndexSafe.Code);
                    break;
                case TemplateConstantType.FullName:
                    code = 'E';
                    result.Assign(this.FullNameSafe.Code);
                    break;
                case TemplateConstantType.IndexPair:
                case TemplateConstantType.IndexTriple:
                case TemplateConstantType.NamedIndex:
                case TemplateConstantType.NamedIndexPair:
                case TemplateConstantType.NamedIndexTriple:
                    if (this.TypeCategory >= TemplateConstantType.NamedIndex)
                        result.Assign(this.FullNameSafe.Code);
                    switch (this.TypeCategory)
                    {
                        case TemplateConstantType.IndexPair:
                        case TemplateConstantType.NamedIndexPair:
                            result.Append(this.Index1Safe.Code);
                            result.Append(this.Index2Safe.Code);
                            break;
                        case TemplateConstantType.IndexTriple:
                        case TemplateConstantType.NamedIndexTriple:
                            result.Append(this.Index1Safe.Code);
                            result.Append(this.Index2Safe.Code);
                            result.Append(this.Index3Safe.Code);
                            break;
                        case TemplateConstantType.NamedIndex:
                            result.Append(this.Index1Safe.Code);
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
                    result.Assign(this.ZNameSafe.Code);
                    result.Append(this.Index1Safe.Code);
                    break;
            }
            return code + result;
        }
    }
}