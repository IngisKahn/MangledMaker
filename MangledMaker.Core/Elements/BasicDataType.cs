namespace MangledMaker.Core.Elements
{
    using System;
    using Attributes;

    public sealed class BasicDataType : ComplexElement
    {
        public enum Types
        {
            SignedChar = 'C',
            Char,
            UnsignedChar,
            Short,
            UnsignedShort,
            Int,
            UnsignedInt,
            Long,
            UnsignedLong,
            Float = 'M',
            Double,
            LongDouble,
            Pointer,
            ConstPointer,
            VolatilePointer,
            ConstVolatilePointer,
            UserDefinedType = -1,
            Extended = '_'
        }

        private ExtendedTypes extendedType;
        private bool isMissing;
        private PointerType? pointerType;

        [Child]
        public PointerType? PointerType
        {
            get
            {
                switch (this.type)
                {
                    case Types.Pointer:
                    case Types.ConstPointer:
                    case Types.VolatilePointer:
                    case Types.ConstVolatilePointer:
                        return
                            this.pointerType ??= new(this, this.SuperType, new());
                    default:
                        return null;
                }
            }
        }
        private PointerTypeArray? pointerTypeArray;

        private Types type;
        private UserDefinedType? userDefinedType;
        private BasicDataType? w64Type;

        public BasicDataType(ComplexElement parent, DecoratedName superType)
            : base(parent) =>
            this.SuperType = superType;

        public unsafe BasicDataType(ComplexElement parent, ref char* pSource,
                                    DecoratedName superType)
            : this(parent, superType) =>
            this.Parse(ref pSource);

        [Input]
        public DecoratedName SuperType { get; set; }

        [Setting]
        public Types Type
        {
            get => this.type;
            set
            {
                this.type = value;
                this.isMissing = false;
            }
        }

        [Setting]
        public ExtendedTypes? ExtendedType
        {
            get => this.type == Types.Extended ? this.extendedType : null;
            set
            {
                this.extendedType = value ?? 0;
                this.isMissing = false;
            }
        }

        [Child]
        public BasicDataType? W64Type => this.type == Types.Extended && this.extendedType == ExtendedTypes.W64 ? this.w64Type ??= new(this, this.SuperType) : null;

        [Child]
        public UserDefinedType? UserDefinedType => this.type == Types.UserDefinedType ? this.userDefinedType ??= new(this) : null;


        [Child]
        public PointerTypeArray? PointerTypeArray =>
            this.type == Types.Extended && this.extendedType == ExtendedTypes.Array
                ? this.pointerTypeArray ??= new(this, this.SuperType, new())
                : null;

        protected override DecoratedName GenerateName()
        {
            if (this.isMissing)
                return new DecoratedName(this, NodeStatus.Truncated) + this.SuperType;
            var pointerOptions = -1;
            DecoratedName result = new(this);
            switch (this.type)
            {
                case Types.SignedChar:
                case Types.Char:
                case Types.UnsignedChar:
                    result.Assign("char");
                    break;
                case Types.Short:
                case Types.UnsignedShort:
                    result.Assign("short");
                    break;
                case Types.Int:
                case Types.UnsignedInt:
                    result.Assign("int");
                    break;
                case Types.Long:
                case Types.UnsignedLong:
                    result.Assign("long");
                    break;
                case Types.Float:
                    result.Assign("float");
                    break;
                case Types.Double:
                    result.Assign("double");
                    break;
                case Types.LongDouble:
                    result.Assign("long double");
                    break;
                case Types.Pointer:
                case Types.ConstPointer:
                case Types.VolatilePointer:
                case Types.ConstVolatilePointer:
                    pointerOptions = (int)this.type & 3;
                    break;
                case Types.Extended:
                    switch (this.extendedType)
                    {
                        case ExtendedTypes.W64:
                            result.Assign("__w64 ");
                            return this.w64Type != null ? result + this.w64Type.Name : result;
                        case ExtendedTypes.Int8:
                        case ExtendedTypes.UnsignedInt8:
                            result.Assign("__int8");
                            break;
                        case ExtendedTypes.Int16:
                        case ExtendedTypes.UnsignedInt16:
                            result.Assign("__int16");
                            break;
                        case ExtendedTypes.Int32:
                        case ExtendedTypes.UnsignedInt32:
                            result.Assign("__int32");
                            break;
                        case ExtendedTypes.Int64:
                        case ExtendedTypes.UnsignedInt64:
                            result.Assign("__int64");
                            break;
                        case ExtendedTypes.Int128:
                        case ExtendedTypes.UnsignedInt128:
                            result.Assign("__int128");
                            break;
                        case ExtendedTypes.Bool:
                            result.Assign("bool");
                            break;
                        case ExtendedTypes.Array:
                            pointerOptions = -2;
                            break;
                        case ExtendedTypes.WCharT:
                            result.Assign("wchar_t");
                            break;
                        default:
                            result.Assign("UNKNOWN");
                            break;
                    }
                    break;
                default:
                    if (this.userDefinedType != null)
                        result.Assign(this.userDefinedType.Name);
                    if (result.IsEmpty)
                        return result;
                    break;
            }

            if (pointerOptions == -1)
            {
                switch (this.type)
                {
                    case Types.SignedChar:
                        result.Prepend("signed ");
                        break;
                    case Types.UnsignedChar:
                    case Types.UnsignedShort:
                    case Types.UnsignedInt:
                    case Types.UnsignedLong:
                        result.Prepend("unsigned ");
                        break;
                    case Types.Extended:
                        switch (this.extendedType)
                        {
                            case ExtendedTypes.UnsignedInt8:
                            case ExtendedTypes.UnsignedInt16:
                            case ExtendedTypes.UnsignedInt32:
                            case ExtendedTypes.UnsignedInt64:
                            case ExtendedTypes.UnsignedInt128:
                                result.Prepend("unsigned ");
                                break;
                        }
                        break;
                }

                if (this.SuperType.IsEmpty)
                    return result;
                result.Append(' ');
                result.Append(this.SuperType);
                return result;
            }
            var cvType = new DecoratedName(this);
            var super = new DecoratedName(this, this.SuperType);
            if (pointerOptions == -2)
            {
                super.IsArray = true;
                if (this.pointerTypeArray == null)
                    throw new InvalidOperationException("Missing pointer type array");
                this.pointerTypeArray.CvType = cvType;
                this.pointerTypeArray.SuperType = this.SuperType;
                var array = this.pointerTypeArray.Name;
                if (!array.IsArray)
                    array.Append("[]");
                return array;
            }
            if (this.SuperType.IsEmpty)
                if ((pointerOptions & 1) != 0)
                {
                    cvType.Assign("const");
                    if ((pointerOptions & 2) != 0)
                        cvType.Append(" volatile");
                }
                else if ((pointerOptions & 2) != 0)
                    cvType.Append("volatile");
            if (this.pointerType == null)
                throw new InvalidOperationException("Missing pointer type");
            this.pointerType.CvType = cvType;
            this.pointerType.SuperType = super;
            return this.pointerType.Name;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.type = (Types)(*pSource);
            if (this.type == 0)
            {
                this.isMissing = true;
                return;
            }

            this.isMissing = false;
            pSource++;

            switch (this.type)
            {
                case Types.Extended:
                    this.extendedType = (ExtendedTypes)(*pSource++);
                    switch (this.extendedType)
                    {
                        case ExtendedTypes.W64:
                            this.w64Type = new(this, ref pSource, this.SuperType);
                            break;
                        case ExtendedTypes.Array:
                            this.pointerTypeArray = new(this, ref pSource,
                                this.SuperType, new());
                            break;
                        case (ExtendedTypes)'X':
                        case (ExtendedTypes)'Y':
                            pSource--;
                            this.type = Types.UserDefinedType;
                            this.userDefinedType = new(this, ref pSource);
                            break;
                        case ExtendedTypes.Int8:
                        case ExtendedTypes.Int16:
                        case ExtendedTypes.Int32:
                        case ExtendedTypes.Int64:
                        case ExtendedTypes.Int128:
                        case ExtendedTypes.Bool:
                        case ExtendedTypes.UnsignedInt8:
                        case ExtendedTypes.UnsignedInt16:
                        case ExtendedTypes.UnsignedInt32:
                        case ExtendedTypes.UnsignedInt64:
                        case ExtendedTypes.UnsignedInt128:
                        case ExtendedTypes.WCharT:
                            break;
                        default:
                            this.extendedType = ExtendedTypes.Unknown;
                            break;
                    }
                    break;
                case Types.Pointer:
                case Types.ConstPointer:
                case Types.VolatilePointer:
                case Types.ConstVolatilePointer:
                    this.pointerType = new PointerType(this, ref pSource, this.SuperType,
                        new());
                    break;
                default:
                    if (Enum.GetName(typeof(Types), this.type) == null)
                    {
                        pSource--;
                        this.type = Types.UserDefinedType;
                        this.userDefinedType = new(this, ref pSource);
                    }
                    break;
            }
        }

        protected override DecoratedName GenerateCode()
        {
            var code = new DecoratedName(this);
            if (this.isMissing)
            {
                code.Assign('\0');
                return code;
            }

            if (this.type == Types.UserDefinedType)
            {
                if (this.userDefinedType == null)
                    throw new InvalidOperationException("missing user defined type");
                var udt = this.userDefinedType.Code;
                var ecsu = udt.ToString()[0];
                if (ecsu is 'X' or 'Y')
                    code.Assign('_');
                code.Append(udt);
                return code;
            }

            code.Assign((char)this.type);
            switch (this.type)
            {
                case Types.Pointer:
                case Types.ConstPointer:
                case Types.VolatilePointer:
                case Types.ConstVolatilePointer:
                    if (this.pointerType != null)
                        code.Append(this.pointerType.Code);
                    break;
                case Types.Extended:
                    code.Append((char)this.extendedType);
                    switch (this.extendedType)
                    {
                        case ExtendedTypes.W64:
                            if (this.w64Type != null)
                                code.Append(this.w64Type.Code);
                            break;
                        case ExtendedTypes.Array:
                            if (this.pointerTypeArray != null)
                                code.Append(this.pointerTypeArray.Code);
                            break;
                    }
                    break;
            }
            return code;
        }

        public enum ExtendedTypes
        {
            Unknown = 'A',
            W64 = '$',
            Int8 = 'D',
            UnsignedInt8,
            Int16,
            UnsignedInt16,
            Int32,
            UnsignedInt32,
            Int64,
            UnsignedInt64,
            Int128,
            UnsignedInt128,
            Bool,
            Array,
            WCharT = 'W'
        }
    }
}