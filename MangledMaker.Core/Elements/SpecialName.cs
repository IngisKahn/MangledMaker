namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class SpecialName : ComplexElement
    {
        private ExternalDataType? externalDataType;
        private ExternalDataType ExternalDataTypeSafe => this.externalDataType ??= new(this, this.Symbol);
        private GuardNumber? guardNumber;
        private GuardNumber GuardNumberSafe => this.guardNumber ??= new(this);

        private VbTableType? virtualBaseTableType;
        private VbTableType VirtualBaseTableTypeSafe => this.virtualBaseTableType ??= new(this, this.Symbol);
        private VdispMapType? virtualDisplacementMapType;
        private VdispMapType VirtualDisplacementMapTypeSafe => this.virtualDisplacementMapType ??= new(this, this.Symbol);

        private VfTableType? virtualFunctionTableType;
        private VfTableType VirtualFunctionTableTypeSafe => this.virtualFunctionTableType ??= new(this, this.Symbol);

        public SpecialName(ComplexElement parent, TypeEncoding typeEncoding,
                           DecoratedName symbol)
            : base(parent)
        {
            this.Symbol = symbol;
            this.TypeEncoding = typeEncoding;
        }

        public unsafe SpecialName(ComplexElement parent, ref char* pSource,
                                  TypeEncoding typeEncoding, DecoratedName symbol)
            : this(parent, typeEncoding, symbol) =>
            this.Parse(ref pSource);

        [Input]
        public DecoratedName Symbol { get; set; }

        [Input]
        public TypeEncoding TypeEncoding { get; set; }

        [Child]
        public VfTableType? VirtualFunctionTableType => this.TypeEncoding.IsVirtualFunctionTable ? this.virtualFunctionTableType : null;

        [Child]
        public VbTableType? VirtualBaseTableType => this.TypeEncoding.IsVirtualBaseTable ? this.virtualBaseTableType : null;

        [Child]
        public GuardNumber? GuardNumber => this.TypeEncoding.IsGuard ? this.guardNumber : null;

        [Child]
        public VdispMapType? VirtualDisplacementMapType => this.TypeEncoding.IsVirtualDisplacementMap ? this.virtualDisplacementMapType : null;

        [Child]
        public ExternalDataType? ExternalDataType =>
            !this.TypeEncoding.IsVirtualFunctionTable &&
            !this.TypeEncoding.IsVirtualBaseTable &&
            !this.TypeEncoding.IsGuard &&
            !this.TypeEncoding.IsVirtualDisplacementMap &&
            !(this.TypeEncoding.IsDataMemberConstructorHelper ||
              this.TypeEncoding.IsDataMemberDestructorHelper) &&
            !(!this.TypeEncoding.IsThunk && !this.TypeEncoding.IsDestructorHelper &&
              this.TypeEncoding.IsName)
                ? this.externalDataType
                : null;

        [Output]
        public bool NeedsModifiers { get; private set; }

        protected override DecoratedName GenerateName()
        {
            this.NeedsModifiers = false;

            DecoratedName declaration = new(this, this.Symbol);
            if (this.TypeEncoding.IsVirtualFunctionTable)
            {
                this.VirtualFunctionTableTypeSafe.SuperType = declaration;
                return this.VirtualFunctionTableTypeSafe.Name;
            }
            if (this.TypeEncoding.IsVirtualBaseTable)
            {
                this.VirtualBaseTableTypeSafe.SuperType = declaration;
                return this.VirtualBaseTableTypeSafe.Name;
            }
            if (this.TypeEncoding.IsGuard)
            {
                declaration.Append('{');
                declaration.Append(this.GuardNumberSafe.Name);
                return declaration + "}'";
            }
            if (this.TypeEncoding.IsVirtualDisplacementMap)
            {
                this.VirtualDisplacementMapTypeSafe.SuperType = declaration;
                return this.VirtualDisplacementMapTypeSafe.Name;
            }

            if (this.TypeEncoding.IsThunk && this.TypeEncoding.IsDestructorHelper)
                declaration.Append("`local static destructor helper'");
            else if (this.TypeEncoding.IsDataMemberConstructorHelper)
                declaration.Append("`template static data member constructor helper'");
            else if (this.TypeEncoding.IsDataMemberDestructorHelper)
                declaration.Append("`template static data member destructor helper'");
            else if (this.TypeEncoding.IsName)
                return declaration; // new DecoratedName(declaration);

            if (this.TypeEncoding.IsDataMemberConstructorHelper ||
                this.TypeEncoding.IsDataMemberDestructorHelper)
                declaration.Prepend(' ');
            else
            {
                this.ExternalDataTypeSafe.SuperType = declaration;
                declaration.Assign(this.ExternalDataTypeSafe.Name);
            }

            this.NeedsModifiers = true;
            return declaration;
        }

        private unsafe void Parse(ref char* pSource)
        {
            if (this.TypeEncoding.IsVirtualFunctionTable)
            {
                this.virtualFunctionTableType = new(this, ref pSource, this.Symbol);
                return;
            }
            if (this.TypeEncoding.IsVirtualBaseTable)
            {
                this.virtualBaseTableType = new(this, ref pSource, this.Symbol);
                return;
            }
            if (this.TypeEncoding.IsGuard)
            {
                this.guardNumber = new(this, ref pSource);
                return;
            }
            if (this.TypeEncoding.IsVirtualDisplacementMap)
            {
                this.virtualDisplacementMapType = new(this, ref pSource, this.Symbol);
                return;
            }

            if (!(this.TypeEncoding.IsThunk && this.TypeEncoding.IsDestructorHelper) &&
                !this.TypeEncoding.IsDataMemberConstructorHelper &&
                !this.TypeEncoding.IsDataMemberDestructorHelper &&
                this.TypeEncoding.IsName)
                return; // new DecoratedName(declaration);

            if (!(this.TypeEncoding.IsDataMemberConstructorHelper ||
                  this.TypeEncoding.IsDataMemberDestructorHelper))
                this.externalDataType = new(this, ref pSource, this.Symbol);
        }

        protected override DecoratedName GenerateCode()
        {
            if (this.TypeEncoding.IsVirtualFunctionTable)
                return this.VirtualFunctionTableTypeSafe.Code;
            if (this.TypeEncoding.IsVirtualBaseTable)
                return this.VirtualBaseTableTypeSafe.Code;
            if (this.TypeEncoding.IsGuard)
                return this.GuardNumberSafe.Code;
            if (this.TypeEncoding.IsVirtualDisplacementMap)
                return this.VirtualDisplacementMapTypeSafe.Code;

            if (!(this.TypeEncoding.IsThunk && this.TypeEncoding.IsDestructorHelper) &&
                !this.TypeEncoding.IsDataMemberConstructorHelper &&
                !this.TypeEncoding.IsDataMemberDestructorHelper &&
                this.TypeEncoding.IsName)
                return new();

            if (!(this.TypeEncoding.IsDataMemberConstructorHelper ||
                  this.TypeEncoding.IsDataMemberDestructorHelper))
                return this.ExternalDataTypeSafe.Code;

            return new();
        }
    }
}