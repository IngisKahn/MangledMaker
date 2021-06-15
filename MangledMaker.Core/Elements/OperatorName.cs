namespace MangledMaker.Core.Elements
{
    using System;
    using Attributes;

    public sealed class OperatorName : ComplexElement
    {
        private static readonly string[] nameTable =
            {
	            " new",
	            " delete",
	            "=",
	            ">>",
	            "<<",
	            "!",
	            "==",
	            "!=",
	            "[]",
	            "operator",
	            "->",
	            "*",
	            "++",
	            "--",
	            "-",
	            "+",
	            "&",
	            "->*",
	            "/",
	            "%",
	            "<",
	            "<=",
	            ">",
	            ">=",
	            ",",
	            "()",
	            "~",
	            "^",
	            "|",
	            "&&",
	            "||",
	            "*=",
	            "+=",
	            "-=",
	            "/=",
	            "%=",
	            ">>=",
	            "<<=",
	            "&=",
	            "|=",
	            "^=",

#if	( !NO_COMPILER_NAMES )
	            "`vftable'",
	            "`vbtable'",
	            "`vcall'",
                "`anonymous namespace'",
	            "`typeof'",
	            "`local static guard'",
	            "`string'",
	            "`vbase destructor'",
	            "`vector deleting destructor'",
	            "`default constructor closure'",
	            "`scalar deleting destructor'",
	            "`vector constructor iterator'",
	            "`vector destructor iterator'",
	            "`vector vbase constructor iterator'",
	            "`virtual displacement map",
	            "`eh vector constructor iterator'",
	            "`eh vector destructor iterator'",
	            "`eh vector vbase constructor iterator'",
	            "`copy constructor closure'",
	            "`udt returning'",
	            "`EH", //eh initialized struct
	            "`RTTI", //rtti initialized struct
	            "`local vftable'",
	            "`local vftable constructor closure'",
#endif	// !NO_COMPILER_NAMES

	            " new[]",
	            " delete[]",

#if ( !NO_COMPILER_NAMES )
	            "`omni callsig'",
	            "`placement delete closure'",
	            "`placement delete[] closure'",
#endif

	            "`managed vector constructor iterator'",
                "`managed vector destructor iterator'",
                "`eh vector copy constructor iterator'",
                "`eh vector vbase copy constructor iterator'",
                "`dynamic initializer for '",
                "`dynamic atexit destructor for '",
                "`vector copy constructor iterator'",
                "`vector vbase copy constructor iterator'",
                "`managed vector copy constructor iterator'",
                "`local static thread guard'"
            };

        private static readonly string[] rttiTable =
            {
                " Type Descriptor'",
	            " Base Class Descriptor at (",
	            " Base Class Array'",
	            " Class Hierarchy Descriptor'",
	            " Complete Object Locator'",
            };

        public enum Extended2OperatorType
        {
            ManagedVectorConstructorIterator = 'A',
            ManagedVectorDestructorIterator,
            EhVectorCopyConstructorIterator,
            EhVectorVirtualBaseCopyConstructorIterator,
            DynamicInitializer,
            DynamicAtExitDestructor,
            VectorCopyConstructorIterator,
            VectorVirtualBaseCopyConstructorIterator,
            ManagedVectorCopyConstructorIterator,
            LocalStaticThreadGuard,
        }

        private static string GetString(Extended2OperatorType type) =>
            OperatorName.nameTable[type - Extended2OperatorType.ManagedVectorConstructorIterator + 
                                   (ExtendedOperatorType.PlacementDeleteVectorClosure - ExtendedOperatorType.TypeOf + 1)
                                   + (ExtendedOperatorType.VirtualCall - ExtendedOperatorType.DivideAssign + 2
                                       + (OperatorType.MinusAssign - OperatorType.Index + 1) + (OperatorType.NotEqual - OperatorType.New + 1))];

        public enum ExtendedOperatorType
        {
            DivideAssign = '0',
            ModulusAssign,
            LeftShiftAssign,
            RightShiftAssign,
            AndAssign,
            OrAssign,
            XorAssign,
            VirtualFunctionTable,
            VirtualBaseTable,
            VirtualCall,
            // ?0
            Namespace = '?',

            TypeOf = 'A',
            LocalStaticGuard,
            String,
            VirtualBaseDestructor,
            VectorDeletingDestructor,
            DefaultConstructorClosure,
            ScalarDeleteingDestructor,
            VectorConstructorIterator,
            VectorDestructorIterator,
            VectorVirtualBaseConstructorIterator,
            VirtualDisplacementMap,
            EhVectorConstructorIterator,
            EhVectorDestructorIterator,
            EhVectorVirtualBaseConstructorIterator,
            CopyConstructorClosure,
            UdtReturning,
            ErrorHandler,
            Rtti,
            LocalVirtualFunctionTable,
            LocalVirtualFunctionTableConstructorClosure,
            NewVector,
            DeleteVector,
            OmniCallSig = 'W',
            PlacementDeleteClosure = 'X',
            PlacementDeleteVectorClosure,
            Extended2 = '_'
        }

        private static string GetString(ExtendedOperatorType type)
        {
            const int offset =
                OperatorType.MinusAssign - OperatorType.Index + 1 + (OperatorType.NotEqual - OperatorType.New) + 1;
            return type switch
            {
                < ExtendedOperatorType.Namespace => OperatorName.nameTable[
                    type - ExtendedOperatorType.DivideAssign + offset],
                ExtendedOperatorType.Namespace => OperatorName.nameTable[
                    ExtendedOperatorType.VirtualCall - ExtendedOperatorType.DivideAssign + 1 + offset],
                _ => OperatorName.nameTable[
                    type - ExtendedOperatorType.TypeOf +
                    (ExtendedOperatorType.VirtualCall - ExtendedOperatorType.DivideAssign) + 2 + offset]
            };
        }

        public enum OperatorType : ushort
        {
            Ctor = '0',
            Dtor,
            New,
            Delete,
            Assign,
            ShiftRight,
            ShiftLeft,
            LogicalNot,
            Equal,
            NotEqual,
            Index = 'A',
            UserDefinedCast,
            DereferencedMember,
            Multiply,
            Increment,
            Decrement,
            Minus,
            Add,
            BinaryAnd,
            DereferencedMemberIndirection,
            Divide,
            Modulus,
            LessThan,
            LessThanOrEqual,
            GreaterThan,
            GreaterThanOrEqual,
            Comma,
            Call,
            BinaryNot,
            BinaryXor,
            BinaryOr,
            LogicalAnd,
            LogicalOr,
            MultiplyAssign,
            AddAssign,
            MinusAssign,
            Extended = '_'
        }

        private static string GetString(OperatorType type) => 
            OperatorName.nameTable[type < OperatorType.Index ? type - OperatorType.New : type - OperatorType.Index + (OperatorType.NotEqual - OperatorType.New) + 1];

        public enum RttiOperatorType
        {
            TypeDescriptor = '0',
            BaseClassDescriptor,
            BaseClassArray,
            ClassHierarchyDescriptor,
            CompleteObjectLocator
        }

        private static string GetString(RttiOperatorType type) => OperatorName.rttiTable[type - RttiOperatorType.TypeDescriptor];

        private Dimension? attributes;
        private Dimension AttributesSafe => this.attributes ??= new(this);
        private ZName? className;
        private ZName ClassNameSafe => this.className ??= new(this, false);
        private SignedDimension? displacementInsideVirtualBaseTable;

        private SignedDimension DisplacementInsideVirtualBaseTableSafe =>
            this.displacementInsideVirtualBaseTable ??= new(this, false);
        private FullName? dynamicOwnerName;
        private FullName DynamicOwnerNameSafe => this.dynamicOwnerName ??= new(this);
        private SymbolName? dynamicOwnerSymbol;
        private SymbolName DynamicOwnerSymbolSafe => this.dynamicOwnerSymbol ??= new(this);
        private Extended2OperatorType extended2Type;
        private ExtendedOperatorType extendedType;
        private bool isDynamicFullName;
        private SignedDimension? memberDisplacement;
        private SignedDimension MemberDisplacementSafe => this.memberDisplacement ??= new(this, false);
        private StringEncoding? namespaceName;
        private StringEncoding NamespaceNameSafe => this.namespaceName ??= new(this, OperatorName.GetString(ExtendedOperatorType.Namespace), "");
        private DataType? rttiDataType;
        private DataType RttiDataTypeSafe => this.rttiDataType ??= new(this, null);
        private RttiOperatorType rttiType;
        private StringEncoding? stringReference;
        private StringEncoding StringReferenceSafe => this.stringReference ??= new(this, OperatorName.GetString(ExtendedOperatorType.String), "");
        private ArgumentList? templateArguments;
        private ArgumentList TemplateArgumentsSafe => this.templateArguments ??= new(this);

        private OperatorType type;
        private OperatorName? udtOperator;
        private OperatorName UdtOperatorSafe => this.udtOperator ??= new(this, false);
        private SignedDimension? virtualBaseTableDisplacement;
        private SignedDimension VirtualBaseTableDisplacementSafe => this.virtualBaseTableDisplacement ??= new(this, false);

        public OperatorName(ComplexElement parent, bool isTemplate, bool isLocked = false)
            : base(parent)
        {
            this.IsTemplate = isTemplate;
            this.IsLocked = isLocked;
        }

        public unsafe OperatorName(ComplexElement parent, ref char* pSource,
                                   bool isTemplate)
            : this(parent, isTemplate) =>
            this.Parse(ref pSource);

        public unsafe OperatorName(ComplexElement parent, ref char* pSource,
                                   bool isTemplate, bool isLocked)
            : this(parent, isTemplate, isLocked) =>
            this.Parse(ref pSource);

        [Input]
        public bool IsLocked { get; private set; }

        [Input]
        public bool IsTemplate { get; private set; }

        [Setting]
        public OperatorType? Type
        {
            get => this.IsLocked ? null : this.type;
            set { if (value != null) this.type = (OperatorType)value; }
        }

        [Setting]
        public ExtendedOperatorType? ExtendedType
        {
            get =>
                this.IsLocked || this.type != OperatorType.Extended
                    ? null
                    : this.extendedType;
            set { if (value != null) this.extendedType = (ExtendedOperatorType)value; }
        }


        [Setting]
        public Extended2OperatorType? Extended2Type
        {
            get =>
                this.IsLocked || this.type != OperatorType.Extended ||
                this.extendedType != ExtendedOperatorType.Extended2
                    ? null
                    : this.extended2Type;
            set { if (value != null) this.extended2Type = (Extended2OperatorType)value; }
        }

        [Setting]
        public RttiOperatorType? RttiType
        {
            get =>
                this.IsLocked || this.type != OperatorType.Extended ||
                this.extendedType != ExtendedOperatorType.Rtti
                    ? null
                    : this.rttiType;
            set { if (value != null) this.rttiType = (RttiOperatorType)value; }
        }

        [Setting]
        public bool? IsDynamicFullName
        {
            get =>
                this.type == OperatorType.Extended
                && this.extendedType == ExtendedOperatorType.Extended2
                && this.extended2Type is Extended2OperatorType.DynamicInitializer or Extended2OperatorType.DynamicAtExitDestructor
                    ? this.isDynamicFullName
                    : (bool?)null;
            set { if (value != null) this.isDynamicFullName = (bool)value; }
        }

        [Output]
        public bool ReadTemplateArguments { get; private set; }

        [Child]
        public ArgumentList? TemplateArguments =>
            (this.type == OperatorType.Ctor || this.type == OperatorType.Dtor)
            && this.IsTemplate
                ? this.templateArguments
                : null;

        [Child]
        public ZName? ClassName =>
            (this.type == OperatorType.Ctor || this.type == OperatorType.Dtor)
                ? this.className
                : null;

        [Child]
        public StringEncoding? NamespaceName =>
            this.type == OperatorType.Extended && this.extendedType == ExtendedOperatorType.Namespace
                ? this.namespaceName
                : null;


        [Child]
        public StringEncoding? StringReference =>
            this.type == OperatorType.Extended && this.extendedType == ExtendedOperatorType.String
                ? this.stringReference
                : null;


        [Child]
        public OperatorName? UdtOperator =>
            this.type == OperatorType.Extended && this.extendedType == ExtendedOperatorType.UdtReturning
                ? this.udtOperator
                : null;

        [Child]
        public DataType? RttiDataType =>
            this.type == OperatorType.Extended
            && this.extendedType == ExtendedOperatorType.Rtti
            && this.rttiType == RttiOperatorType.TypeDescriptor
                ? this.rttiDataType
                : null;

        [Child]
        public SignedDimension? MemberDisplacement =>
            this.type == OperatorType.Extended
            && this.extendedType == ExtendedOperatorType.Rtti
            && this.rttiType == RttiOperatorType.BaseClassDescriptor
                ? this.memberDisplacement
                : null;

        [Child]
        public SignedDimension? VirtualBaseTableDisplacement =>
            this.type == OperatorType.Extended
            && this.extendedType == ExtendedOperatorType.Rtti
            && this.rttiType == RttiOperatorType.BaseClassDescriptor
                ? this.virtualBaseTableDisplacement
                : null;

        [Child]
        public SignedDimension? DisplacementInsideVirtualBaseTable =>
            this.type == OperatorType.Extended
            && this.extendedType == ExtendedOperatorType.Rtti
            && this.rttiType == RttiOperatorType.BaseClassDescriptor
                ? this.displacementInsideVirtualBaseTable
                : null;

        [Child]
        public Dimension? Attributes =>
            this.type == OperatorType.Extended
            && this.extendedType == ExtendedOperatorType.Rtti
            && this.rttiType == RttiOperatorType.BaseClassDescriptor
                ? this.attributes
                : null;

        [Child]
        public FullName? DynamicOwnerName =>
            this.type == OperatorType.Extended
            && this.extendedType == ExtendedOperatorType.Extended2
            && (this.extended2Type is Extended2OperatorType.DynamicInitializer or Extended2OperatorType.DynamicAtExitDestructor)
            && this.isDynamicFullName
                ? this.dynamicOwnerName
                : null;

        [Child]
        public SymbolName? DynamicOwnerSymbol =>
            this.type == OperatorType.Extended
            && this.extendedType == ExtendedOperatorType.Extended2
            && (this.extended2Type is Extended2OperatorType.DynamicInitializer or Extended2OperatorType.DynamicAtExitDestructor)
            && !this.isDynamicFullName
                ? this.dynamicOwnerSymbol
                : null;



        protected override DecoratedName GenerateName()
        {
            DecoratedName operatorName = new(this);
            DecoratedName tempName = new(this);
            var udcSeen = false;
            this.ReadTemplateArguments = false;

            switch (this.type)
            {
                case OperatorType.Ctor:
                case OperatorType.Dtor:
                    DecoratedName list = new(this);
                    if (this.IsTemplate)
                    {
                        list.Assign('<');
                        list.Append(this.TemplateArgumentsSafe.Name);
                        if (list.LastCharacter == '>')
                            list.Append(' ');
                        list.Append('>');
                        this.ReadTemplateArguments = true;
                        if (this.ClassNameSafe.Name.IsEmpty)
                            return list;
                    }
                    operatorName.Assign(this.ClassNameSafe.Name);
                    if (this.type == OperatorType.Dtor)
                        operatorName.Prepend('~');
                    if (!list.IsEmpty)
                        operatorName.Append(list);
                    return operatorName;
                case OperatorType.UserDefinedCast:
                    udcSeen = true;
                    goto case OperatorType.New;
                case OperatorType.New:
                case OperatorType.Delete:
                case OperatorType.Assign:
                case OperatorType.ShiftRight:
                case OperatorType.ShiftLeft:
                case OperatorType.LogicalNot:
                case OperatorType.Equal:
                case OperatorType.NotEqual:
                case OperatorType.Index:
                case OperatorType.DereferencedMember:
                case OperatorType.Multiply:
                case OperatorType.Increment:
                case OperatorType.Decrement:
                case OperatorType.Minus:
                case OperatorType.Add:
                case OperatorType.BinaryAnd:
                case OperatorType.DereferencedMemberIndirection:
                case OperatorType.Divide:
                case OperatorType.Modulus:
                case OperatorType.LessThan:
                case OperatorType.LessThanOrEqual:
                case OperatorType.GreaterThan:
                case OperatorType.GreaterThanOrEqual:
                case OperatorType.Comma:
                case OperatorType.Call:
                case OperatorType.BinaryNot:
                case OperatorType.BinaryXor:
                case OperatorType.BinaryOr:
                case OperatorType.LogicalAnd:
                case OperatorType.LogicalOr:
                case OperatorType.MultiplyAssign:
                case OperatorType.AddAssign:
                case OperatorType.MinusAssign:
                    operatorName.Assign(OperatorName.GetString(this.type));
                    break;
                case OperatorType.Extended:
                    switch (this.extendedType)
                    {
                        case ExtendedOperatorType.DivideAssign:
                        case ExtendedOperatorType.ModulusAssign:
                        case ExtendedOperatorType.RightShiftAssign:
                        case ExtendedOperatorType.LeftShiftAssign:
                        case ExtendedOperatorType.AndAssign:
                        case ExtendedOperatorType.OrAssign:
                        case ExtendedOperatorType.XorAssign:
                            operatorName.Assign(GetString(this.extendedType));
                            break;
                        case ExtendedOperatorType.VirtualFunctionTable:
                        case ExtendedOperatorType.VirtualBaseTable:
                            return new(this, OperatorName.GetString(this.extendedType));
                        case ExtendedOperatorType.VirtualCall:
                            return new(this, OperatorName.GetString(this.extendedType)) { IsVirtualCallThunk = true };
                        case ExtendedOperatorType.Namespace:
                            var @namespace = this.NamespaceNameSafe.Name;
                            @namespace.IsNoTypeEncoding = true;
                            return @namespace;
                        case ExtendedOperatorType.String:
                            var text = this.StringReferenceSafe.Name;
                            text.IsNoTypeEncoding = true;
                            return text;
                        case ExtendedOperatorType.TypeOf:
                        case ExtendedOperatorType.LocalStaticGuard:
                        case ExtendedOperatorType.VirtualBaseDestructor:
                        case ExtendedOperatorType.VectorDeletingDestructor:
                        case ExtendedOperatorType.DefaultConstructorClosure:
                        case ExtendedOperatorType.ScalarDeleteingDestructor:
                        case ExtendedOperatorType.VectorConstructorIterator:
                        case ExtendedOperatorType.VectorDestructorIterator:
                        case ExtendedOperatorType.VectorVirtualBaseConstructorIterator:
                        case ExtendedOperatorType.VirtualDisplacementMap:
                        case ExtendedOperatorType.EhVectorConstructorIterator:
                        case ExtendedOperatorType.EhVectorDestructorIterator:
                        case ExtendedOperatorType.EhVectorVirtualBaseConstructorIterator:
                        case ExtendedOperatorType.CopyConstructorClosure:
                        case ExtendedOperatorType.LocalVirtualFunctionTable:
                        case ExtendedOperatorType.LocalVirtualFunctionTableConstructorClosure:
                        case ExtendedOperatorType.PlacementDeleteClosure:
                        case ExtendedOperatorType.PlacementDeleteVectorClosure:
                            return new(this, OperatorName.GetString(this.extendedType));
                        case ExtendedOperatorType.UdtReturning:
                            operatorName.Assign(GetString(this.extendedType));
                            tempName.Assign(this.UdtOperatorSafe.Name);
                            if (tempName.IsUdtThunk)
                                return new(this, NodeStatus.Invalid);
                            return operatorName + tempName;
                        case ExtendedOperatorType.ErrorHandler: //yup, do nothing
                            break;
                        case ExtendedOperatorType.Rtti:
                            operatorName.Assign("`RTTI");
                            switch (this.rttiType)
                            {
                                case RttiOperatorType.TypeDescriptor:
                                    tempName.Assign(GetString(this.rttiType));
                                    operatorName.Prepend(' ');
                                    return this.RttiDataTypeSafe.Name + operatorName + tempName;
                                case RttiOperatorType.BaseClassDescriptor:
                                    tempName.Assign(GetString(this.rttiType));
                                    var descriptor = operatorName + tempName;
                                    descriptor.Append(this.MemberDisplacementSafe.Name);
                                    descriptor.Append(',');
                                    descriptor.Append(this.VirtualBaseTableDisplacementSafe.Name + ',');
                                    descriptor.Append(',');
                                    descriptor.Append(this.DisplacementInsideVirtualBaseTableSafe.Name + ',');
                                    descriptor.Append(',');
                                    descriptor.Append(this.AttributesSafe.Name);
                                    return descriptor + ")'";
                                case RttiOperatorType.BaseClassArray:
                                case RttiOperatorType.ClassHierarchyDescriptor:
                                case RttiOperatorType.CompleteObjectLocator:
                                    tempName.Assign(OperatorName.GetString(this.rttiType));
                                    return operatorName + tempName;
                            }
                            break;
                        case ExtendedOperatorType.NewVector:
                        case ExtendedOperatorType.DeleteVector:
                            operatorName.Assign(GetString(this.extendedType));
                            break;
                        case ExtendedOperatorType.Extended2:
                            switch (this.extended2Type)
                            {
                                case Extended2OperatorType.ManagedVectorConstructorIterator:
                                case Extended2OperatorType.ManagedVectorDestructorIterator:
                                case Extended2OperatorType.EhVectorCopyConstructorIterator:
                                case Extended2OperatorType.EhVectorVirtualBaseCopyConstructorIterator:
                                case Extended2OperatorType.VectorCopyConstructorIterator:
                                case Extended2OperatorType.VectorVirtualBaseCopyConstructorIterator:
                                case Extended2OperatorType.ManagedVectorCopyConstructorIterator:
                                case Extended2OperatorType.LocalStaticThreadGuard:
                                    return new(this, OperatorName.GetString(this.extended2Type));
                                case Extended2OperatorType.DynamicInitializer:
                                case Extended2OperatorType.DynamicAtExitDestructor:
                                    var dynamic =
                                        new DecoratedName(this, OperatorName.GetString(this.extended2Type));
                                    dynamic.Append(this.isDynamicFullName
                                                       ? this.DynamicOwnerNameSafe.Name
                                                       : this.DynamicOwnerSymbolSafe.Name);
                                    dynamic.Append("''");
                                    return dynamic;
                            }
                            break;
                    }
                    break;
            }
            if (udcSeen)
                operatorName.IsUdc = true;
            else if (!operatorName.IsEmpty)
                operatorName.Prepend("operator");

            return operatorName;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.ReadTemplateArguments = false;

            //	So what type of operator is it ?
            this.type = (OperatorType)(*pSource++);
            switch (this.type)
            {
                case (OperatorType)'\0':
                    pSource--;
                    this.IsTruncated = true;
                    break;
                case OperatorType.Ctor:
                case OperatorType.Dtor:
                    // The constructor and destructor are special:
                    // Their operator name is the name of their first enclosing scope, which
                    // will always be a tag, which may be a template specialization!
                    var savedSource = pSource;
                    if (this.IsTemplate)
                    {
                        this.templateArguments = new(this, ref pSource);
                        this.ReadTemplateArguments = true;
                        if (*pSource == '\0')
                        {
                            this.className = null;
                            break;
                        }
                        pSource++;
                    }
                    this.className = new(this, ref pSource, false);
                    pSource = savedSource;
                    break;
                case OperatorType.Extended:
                    this.extendedType = (ExtendedOperatorType)(*pSource++);
                    switch (this.extendedType)
                    {
                        case (ExtendedOperatorType)'\0':
                            pSource--;
                            this.IsTruncated = true;
                            break;
                        case ExtendedOperatorType.Namespace:
                            switch (*pSource++)
                            {
                                case '\0':
                                    pSource--;
                                    this.IsTruncated = true;
                                    break;
                                case '0':
                                    this.namespaceName = new(this, ref pSource, "`anonymous namespace'");
                                    break;
                                default:
                                    this.IsInvalid = true;
                                    break;
                            }
                            break;
                        case ExtendedOperatorType.String:
                            this.stringReference = new(this, ref pSource, "`string'");
                            break;
                        case ExtendedOperatorType.UdtReturning:
                            this.udtOperator = new(this, ref pSource, false);
                            if (this.udtOperator.Name.IsUdtThunk)
                                this.IsInvalid = true;
                            break;
                        case ExtendedOperatorType.Rtti:
                            this.rttiType = (RttiOperatorType)(*pSource++);
                            switch (this.rttiType)
                            {
                                case RttiOperatorType.TypeDescriptor:
                                    this.rttiDataType = new(this, ref pSource, null);
                                    break;
                                case RttiOperatorType.BaseClassDescriptor:
                                    this.memberDisplacement = new(this, ref pSource);
                                    this.virtualBaseTableDisplacement = new(this, ref pSource);
                                    this.displacementInsideVirtualBaseTable = new(this, ref pSource);
                                    this.attributes = new(this, ref pSource, true);
                                    break;
                                case RttiOperatorType.BaseClassArray:
                                case RttiOperatorType.ClassHierarchyDescriptor:
                                case RttiOperatorType.CompleteObjectLocator:
                                    break;
                                default:
                                    pSource--;
                                    this.IsTruncated = true;
                                    break;
                            }
                            break;
                        case ExtendedOperatorType.Extended2:
                            this.extended2Type = (Extended2OperatorType)(*pSource++);
                            switch (this.extended2Type)
                            {
                                case Extended2OperatorType.DynamicInitializer:
                                case Extended2OperatorType.DynamicAtExitDestructor:
                                    // ReSharper disable once AssignmentInConditionalExpression
                                    if (this.isDynamicFullName = *pSource == '?')
                                    {
                                        this.dynamicOwnerName = new(this, ref pSource);
                                        if (*pSource == '@')
                                            pSource++;
                                    }
                                    else
                                        this.dynamicOwnerSymbol = new(this, ref pSource);
                                    break;
                                default:
                                    if (Enum.IsDefined(typeof(Extended2OperatorType), this.extended2Type))
                                        this.IsInvalid = true;
                                    break;
                            }
                            break;
                        default:
                            if (!Enum.IsDefined(typeof(ExtendedOperatorType), this.extendedType))
                                this.IsInvalid = true;
                            break;
                    }
                    break;
                default:
                    if (!Enum.IsDefined(typeof(OperatorType), this.type))
                        this.IsInvalid = true;
                    break;
            }
        }

        protected override DecoratedName GenerateCode()
        {
            var code = new DecoratedName(this);

            switch (this.type)
            {
                case OperatorType.Ctor:
                case OperatorType.Dtor:
                    if (!this.IsTemplate) 
                        return code + this.ClassNameSafe.Code;
                    code.Append(this.TemplateArgumentsSafe.Code);
                    code.Append('@');
                    return code + this.ClassNameSafe.Code;
                case OperatorType.Extended:
                    code.Append((char)this.extendedType);
                    switch (this.extendedType)
                    {
                        case ExtendedOperatorType.Namespace:
                            code.Append('0');
                            return code + this.NamespaceNameSafe.Code;
                        case ExtendedOperatorType.String:
                            return code + this.StringReferenceSafe.Code;
                        case ExtendedOperatorType.UdtReturning:
                            return code + this.UdtOperatorSafe.Code;
                        case ExtendedOperatorType.Rtti:
                            code.Append((char)this.rttiType);
                            switch (this.rttiType)
                            {
                                case RttiOperatorType.TypeDescriptor:
                                    return code + this.RttiDataTypeSafe.Code;
                                case RttiOperatorType.BaseClassDescriptor:
                                    code.Append(this.MemberDisplacementSafe.Code);
                                    code.Append(this.VirtualBaseTableDisplacementSafe.Code);
                                    code.Append(this.DisplacementInsideVirtualBaseTableSafe.Code);
                                    return code + this.AttributesSafe.Code;
                            }
                            break;
                        case ExtendedOperatorType.Extended2:
                            code.Append((char)this.extended2Type);
                            switch (this.extended2Type)
                            {
                                case Extended2OperatorType.DynamicInitializer:
                                case Extended2OperatorType.DynamicAtExitDestructor:
                                    if (!this.isDynamicFullName) 
                                        return code + this.DynamicOwnerSymbolSafe.Code;
                                    code.Append(this.DynamicOwnerNameSafe.Code);
                                    return code + '@';
                            }
                            break;
                    }
                    break;
            }

            return code;
        }
    }
}