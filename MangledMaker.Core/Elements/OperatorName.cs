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

        private static string GetString(Extended2OperatorType type)
        {
            return nameTable[type - Extended2OperatorType.ManagedVectorConstructorIterator + 
                (ExtendedOperatorType.PlacementDeleteVectorClosure - ExtendedOperatorType.TypeOf + 1)
                + (ExtendedOperatorType.VirtualCall - ExtendedOperatorType.DivideAssign + 2
                + (OperatorType.MinusAssign - OperatorType.Index + 1) + (OperatorType.NotEqual - OperatorType.New + 1))];
        }

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
                (OperatorType.MinusAssign - OperatorType.Index + 1) + (OperatorType.NotEqual - OperatorType.New + 1);
            if (type < ExtendedOperatorType.Namespace)
                return nameTable[type - ExtendedOperatorType.DivideAssign + offset];
            if (type == ExtendedOperatorType.Namespace)
                return nameTable[ExtendedOperatorType.VirtualCall - ExtendedOperatorType.DivideAssign + 1 + offset];
            return
                nameTable[
                          type - ExtendedOperatorType.TypeOf +
                          (ExtendedOperatorType.VirtualCall - ExtendedOperatorType.DivideAssign + 2) + offset];
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

        private static string GetString(OperatorType type)
        {
            return nameTable[type < OperatorType.Index ? type - OperatorType.New : type - OperatorType.Index + (OperatorType.NotEqual - OperatorType.New + 1)];
        }

        public enum RttiOperatorType
        {
            TypeDescriptor = '0',
            BaseClassDescriptor,
            BaseClassArray,
            ClassHierarchyDescriptor,
            CompleteObjectLocator
        }

        private static string GetString(RttiOperatorType type)
        {
            return rttiTable[type - RttiOperatorType.TypeDescriptor];
        }

        private Dimension attributes;
        private ZName className;
        private SignedDimension displacementInsideVirtualBaseTable;
        private FullName dynamicOwnerName;
        private SymbolName dynamicOwnerSymbol;
        private Extended2OperatorType extended2Type;
        private ExtendedOperatorType extendedType;
        private bool isDynamicFullName;
        private SignedDimension memberDisplacement;
        private StringEncoding namespaceName;
        private DataType rttiDataType;
        private RttiOperatorType rttiType;
        private StringEncoding stringReference;
        private ArgumentList templateArguments;

        private OperatorType type;
        private OperatorName udtOperator;
        private SignedDimension virtualBaseTableDisplacement;

        public OperatorName(ComplexElement parent, bool isTemplate, bool isLocked = false)
            : base(parent)
        {
            this.IsTemplate = isTemplate;
            this.IsLocked = isLocked;
        }

        public unsafe OperatorName(ComplexElement parent, ref char* pSource,
                                   bool isTemplate)
            : this(parent, isTemplate)
        {
            this.Parse(ref pSource);
        }

        public unsafe OperatorName(ComplexElement parent, ref char* pSource,
                                   bool isTemplate, bool isLocked)
            : this(parent, isTemplate, isLocked)
        {
            this.Parse(ref pSource);
        }

        [Input]
        public bool IsLocked { get; private set; }

        [Input]
        public bool IsTemplate { get; private set; }

        [Setting]
        public OperatorType? Type
        {
            get { return this.IsLocked ? (OperatorType?)null : this.type; }
            set { if (value != null) this.type = (OperatorType)value; }
        }

        [Setting]
        public ExtendedOperatorType? ExtendedType
        {
            get
            {
                return this.IsLocked || this.type != OperatorType.Extended
                           ? (ExtendedOperatorType?)null
                           : this.extendedType;
            }
            set { if (value != null) this.extendedType = (ExtendedOperatorType)value; }
        }


        [Setting]
        public Extended2OperatorType? Extended2Type
        {
            get
            {
                return this.IsLocked || this.type != OperatorType.Extended ||
                       this.extendedType != ExtendedOperatorType.Extended2
                           ? (Extended2OperatorType?)null
                           : this.extended2Type;
            }
            set { if (value != null) this.extended2Type = (Extended2OperatorType)value; }
        }

        [Setting]
        public RttiOperatorType? RttiType
        {
            get
            {
                return this.IsLocked || this.type != OperatorType.Extended ||
                       this.extendedType != ExtendedOperatorType.Rtti
                           ? (RttiOperatorType?)null
                           : this.rttiType;
            }
            set { if (value != null) this.rttiType = (RttiOperatorType)value; }
        }

        [Setting]
        public bool? IsDynamicFullName
        {
            get
            {
                return this.type == OperatorType.Extended
                       && this.extendedType == ExtendedOperatorType.Extended2
                       && (this.extended2Type == Extended2OperatorType.DynamicInitializer
                           || this.extended2Type == Extended2OperatorType.DynamicAtExitDestructor)
                           ? this.isDynamicFullName
                           : (bool?)null;
            }
            set { if (value != null) this.isDynamicFullName = (bool)value; }
        }

        [Output]
        public bool ReadTemplateArguments { get; private set; }

        [Child]
        public ArgumentList TemplateArguments
        {
            get
            {
                return (this.type == OperatorType.Ctor || this.type == OperatorType.Dtor)
                       && this.IsTemplate
                           ? this.templateArguments
                           : null;
            }
        }

        [Child]
        public ZName ClassName
        {
            get
            {
                return (this.type == OperatorType.Ctor || this.type == OperatorType.Dtor)
                           ? this.className
                           : null;
            }
        }

        [Child]
        public StringEncoding NamespaceName
        {
            get
            {
                return this.type == OperatorType.Extended && this.extendedType == ExtendedOperatorType.Namespace
                           ? this.namespaceName
                           : null;
            }
        }


        [Child]
        public StringEncoding StringReference
        {
            get
            {
                return this.type == OperatorType.Extended && this.extendedType == ExtendedOperatorType.String
                           ? this.stringReference
                           : null;
            }
        }


        [Child]
        public OperatorName UdtOperator
        {
            get
            {
                return this.type == OperatorType.Extended && this.extendedType == ExtendedOperatorType.UdtReturning
                           ? this.udtOperator
                           : null;
            }
        }

        [Child]
        public DataType RttiDataType
        {
            get
            {
                return this.type == OperatorType.Extended
                       && this.extendedType == ExtendedOperatorType.Rtti
                       && this.rttiType == RttiOperatorType.TypeDescriptor
                           ? this.rttiDataType
                           : null;
            }
        }

        [Child]
        public SignedDimension MemberDisplacement
        {
            get
            {
                return this.type == OperatorType.Extended
                       && this.extendedType == ExtendedOperatorType.Rtti
                       && this.rttiType == RttiOperatorType.BaseClassDescriptor
                           ? this.memberDisplacement
                           : null;
            }
        }

        [Child]
        public SignedDimension VirtualBaseTableDisplacement
        {
            get
            {
                return this.type == OperatorType.Extended
                       && this.extendedType == ExtendedOperatorType.Rtti
                       && this.rttiType == RttiOperatorType.BaseClassDescriptor
                           ? this.virtualBaseTableDisplacement
                           : null;
            }
        }

        [Child]
        public SignedDimension DisplacementInsideVirtualBaseTable
        {
            get
            {
                return this.type == OperatorType.Extended
                       && this.extendedType == ExtendedOperatorType.Rtti
                       && this.rttiType == RttiOperatorType.BaseClassDescriptor
                           ? this.displacementInsideVirtualBaseTable
                           : null;
            }
        }

        [Child]
        public Dimension Attributes
        {
            get
            {
                return this.type == OperatorType.Extended
                       && this.extendedType == ExtendedOperatorType.Rtti
                       && this.rttiType == RttiOperatorType.BaseClassDescriptor
                           ? this.attributes
                           : null;
            }
        }

        [Child]
        public FullName DynamicOwnerName
        {
            get
            {
                return this.type == OperatorType.Extended
                       && this.extendedType == ExtendedOperatorType.Extended2
                       && (this.extended2Type == Extended2OperatorType.DynamicInitializer
                           || this.extended2Type == Extended2OperatorType.DynamicAtExitDestructor)
                       && this.isDynamicFullName
                           ? this.dynamicOwnerName
                           : null;
            }
        }

        [Child]
        public SymbolName DynamicOwnerSymbol
        {
            get
            {
                return this.type == OperatorType.Extended
                       && this.extendedType == ExtendedOperatorType.Extended2
                       && (this.extended2Type == Extended2OperatorType.DynamicInitializer
                           || this.extended2Type == Extended2OperatorType.DynamicAtExitDestructor)
                       && !this.isDynamicFullName
                           ? this.dynamicOwnerSymbol
                           : null;
            }
        }

        protected override void CreateEmptyElements()
        {
            if (this.templateArguments == null)
                this.templateArguments = new ArgumentList(this);
            if (this.className == null)
                this.className = new ZName(this, false);
            if (this.namespaceName == null)
                this.namespaceName = new StringEncoding(this, GetString(ExtendedOperatorType.Namespace), "");
            if (this.stringReference == null)
                this.stringReference = new StringEncoding(this, GetString(ExtendedOperatorType.String), "");
            if (this.udtOperator == null)
                this.udtOperator = new OperatorName(this, false);
            if (this.rttiDataType == null)
                this.rttiDataType = new DataType(this, null);
            if (this.memberDisplacement == null)
                this.memberDisplacement = new SignedDimension(this, false);
            if (this.virtualBaseTableDisplacement == null)
                this.virtualBaseTableDisplacement = new SignedDimension(this, false);
            if (this.displacementInsideVirtualBaseTable == null)
                this.displacementInsideVirtualBaseTable = new SignedDimension(this, false);
            if (this.attributes == null)
                this.attributes = new Dimension(this);
            if (this.dynamicOwnerName == null)
                this.dynamicOwnerName = new FullName(this);
            if (this.dynamicOwnerSymbol == null)
                this.dynamicOwnerSymbol = new SymbolName(this);
        }

        protected override DecoratedName GenerateName()
        {
            var operatorName = new DecoratedName(this);
            var tempName = new DecoratedName(this);
            var udcSeen = false;
            this.ReadTemplateArguments = false;

            switch (this.type)
            {
                case OperatorType.Ctor:
                case OperatorType.Dtor:
                    var list = new DecoratedName(this);
                    if (this.IsTemplate)
                    {
                        list.Assign('<');
                        list.Append(this.templateArguments.Name);
                        if (list.LastCharacter == '>')
                            list.Append(' ');
                        list.Append('>');
                        this.ReadTemplateArguments = true;
                        if (this.className.Name.IsEmpty)
                            return list;
                    }
                    operatorName.Assign(this.className.Name);
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
                    operatorName.Assign(GetString(this.type));
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
                            return new DecoratedName(this, GetString(this.extendedType));
                        case ExtendedOperatorType.VirtualCall:
                            return new DecoratedName(this, GetString(this.extendedType)) { IsVirtualCallThunk = true };
                        case ExtendedOperatorType.Namespace:
                            var @namespace = this.namespaceName.Name;
                            @namespace.IsNoTypeEncoding = true;
                            return @namespace;
                        case ExtendedOperatorType.String:
                            var text = this.stringReference.Name;
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
                            return new DecoratedName(this, GetString(this.extendedType));
                        case ExtendedOperatorType.UdtReturning:
                            operatorName.Assign(GetString(this.extendedType));
                            tempName.Assign(this.udtOperator.Name);
                            if (tempName.IsUdtThunk)
                                return new DecoratedName(this, NodeStatus.Invalid);
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
                                    return this.rttiDataType.Name + operatorName + tempName;
                                case RttiOperatorType.BaseClassDescriptor:
                                    tempName.Assign(GetString(this.rttiType));
                                    var descriptor = operatorName + tempName;
                                    descriptor.Append(this.memberDisplacement.Name);
                                    descriptor.Append(',');
                                    descriptor.Append(this.virtualBaseTableDisplacement.Name + ',');
                                    descriptor.Append(',');
                                    descriptor.Append(this.displacementInsideVirtualBaseTable.Name + ',');
                                    descriptor.Append(',');
                                    descriptor.Append(this.attributes.Name);
                                    return descriptor + ")'";
                                case RttiOperatorType.BaseClassArray:
                                case RttiOperatorType.ClassHierarchyDescriptor:
                                case RttiOperatorType.CompleteObjectLocator:
                                    tempName.Assign(GetString(this.rttiType));
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
                                    return new DecoratedName(this, GetString(this.extended2Type));
                                case Extended2OperatorType.DynamicInitializer:
                                case Extended2OperatorType.DynamicAtExitDestructor:
                                    var dynamic =
                                        new DecoratedName(this, GetString(this.extended2Type));
                                    dynamic.Append(this.isDynamicFullName
                                                       ? this.dynamicOwnerName.Name
                                                       : this.dynamicOwnerSymbol.Name);
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
                        this.templateArguments = new ArgumentList(this, ref pSource);
                        this.ReadTemplateArguments = true;
                        if (*pSource == '\0')
                        {
                            this.className = null;
                            break;
                        }
                        pSource++;
                    }
                    this.className = new ZName(this, ref pSource, false);
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
                                    this.namespaceName = new StringEncoding(this, ref pSource, "`anonymous namespace'");
                                    break;
                                default:
                                    this.IsInvalid = true;
                                    break;
                            }
                            break;
                        case ExtendedOperatorType.String:
                            this.stringReference = new StringEncoding(this, ref pSource, "`string'");
                            break;
                        case ExtendedOperatorType.UdtReturning:
                            this.udtOperator = new OperatorName(this, ref pSource, false);
                            if (this.udtOperator.Name.IsUdtThunk)
                                this.IsInvalid = true;
                            break;
                        case ExtendedOperatorType.Rtti:
                            this.rttiType = (RttiOperatorType)(*pSource++);
                            switch (this.rttiType)
                            {
                                case RttiOperatorType.TypeDescriptor:
                                    this.rttiDataType = new DataType(this, ref pSource, null);
                                    break;
                                case RttiOperatorType.BaseClassDescriptor:
                                    this.memberDisplacement = new SignedDimension(this, ref pSource);
                                    this.virtualBaseTableDisplacement = new SignedDimension(this, ref pSource);
                                    this.displacementInsideVirtualBaseTable = new SignedDimension(this, ref pSource);
                                    this.attributes = new Dimension(this, ref pSource, true);
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
#pragma warning disable 665
                                    if (this.isDynamicFullName = *pSource == '?')
#pragma warning restore 665
                                    {
                                        this.dynamicOwnerName = new FullName(this, ref pSource);
                                        if (*pSource == '@')
                                            pSource++;
                                    }
                                    else
                                        this.dynamicOwnerSymbol = new SymbolName(this, ref pSource);
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
                    if (this.IsTemplate)
                    {
                        code.Append(this.templateArguments.Code);
                        code.Append('@');
                    }
                    return code + this.className.Code;
                case OperatorType.Extended:
                    code.Append((char)this.extendedType);
                    switch (this.extendedType)
                    {
                        case ExtendedOperatorType.Namespace:
                            code.Append('0');
                            return code + this.namespaceName.Code;
                        case ExtendedOperatorType.String:
                            return code + this.stringReference.Code;
                        case ExtendedOperatorType.UdtReturning:
                            return code + this.udtOperator.Code;
                        case ExtendedOperatorType.Rtti:
                            code.Append((char)this.rttiType);
                            switch (this.rttiType)
                            {
                                case RttiOperatorType.TypeDescriptor:
                                    return code + this.rttiDataType.Code;
                                case RttiOperatorType.BaseClassDescriptor:
                                    code.Append(this.memberDisplacement.Code);
                                    code.Append(this.virtualBaseTableDisplacement.Code);
                                    code.Append(this.displacementInsideVirtualBaseTable.Code);
                                    return code + this.attributes.Code;
                            }
                            break;
                        case ExtendedOperatorType.Extended2:
                            code.Append((char)this.extended2Type);
                            switch (this.extended2Type)
                            {
                                case Extended2OperatorType.DynamicInitializer:
                                case Extended2OperatorType.DynamicAtExitDestructor:
                                    if (!this.isDynamicFullName) 
                                        return code + this.dynamicOwnerSymbol.Code;
                                    code.Append(this.dynamicOwnerName.Code);
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