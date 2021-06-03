namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class SpecialName : ComplexElement
    {
        private ExternalDataType externalDataType;
        private GuardNumber guardNumber;

        private VbTableType virtualBaseTableType;
        private VdispMapType virtualDisplacementMapType;

        private VfTableType virtualFunctionTableType;

        public SpecialName(ComplexElement parent, TypeEncoding typeEncoding,
                           DecoratedName symbol)
            : base(parent)
        {
            this.Symbol = symbol;
            this.TypeEncoding = typeEncoding;
        }

        public unsafe SpecialName(ComplexElement parent, ref char* pSource,
                                  TypeEncoding typeEncoding, DecoratedName symbol)
            : this(parent, typeEncoding, symbol)
        {
            this.Parse(ref pSource);
        }

        [Input]
        public DecoratedName Symbol { get; set; }

        [Input]
        public TypeEncoding TypeEncoding { get; set; }

        [Child]
        public VfTableType VirtualFunctionTableType
        {
            get { return this.TypeEncoding.IsVirtualFunctionTable ? this.virtualFunctionTableType : null; }
        }

        [Child]
        public VbTableType VirtualBaseTableType
        {
            get { return this.TypeEncoding.IsVirtualBaseTable ? this.virtualBaseTableType : null; }
        }

        [Child]
        public GuardNumber GuardNumber
        {
            get { return this.TypeEncoding.IsGuard ? this.guardNumber : null; }
        }

        [Child]
        public VdispMapType VirtualDisplacementMapType
        {
            get { return this.TypeEncoding.IsVirtualDisplacementMap ? this.virtualDisplacementMapType : null; }
        }

        [Child]
        public ExternalDataType ExternalDataType
        {
            get
            {
                if (!this.TypeEncoding.IsVirtualFunctionTable &&
                    !this.TypeEncoding.IsVirtualBaseTable &&
                    !this.TypeEncoding.IsGuard &&
                    !this.TypeEncoding.IsVirtualDisplacementMap &&
                    !(this.TypeEncoding.IsDataMemberConstructorHelper ||
                      this.TypeEncoding.IsDataMemberDestructorHelper) &&
                    !(!this.TypeEncoding.IsThunk && !this.TypeEncoding.IsDestructorHelper &&
                      this.TypeEncoding.IsName)
                    )
                    return this.externalDataType;
                return null;
            }
        }

        [Output]
        public bool NeedsModifiers { get; private set; }

        protected override void CreateEmptyElements()
        {
            if (this.virtualFunctionTableType == null)
                this.virtualFunctionTableType = new VfTableType(this, this.Symbol);
            if (this.virtualBaseTableType == null)
                this.virtualBaseTableType = new VbTableType(this, this.Symbol);
            if (this.guardNumber == null)
                this.guardNumber = new GuardNumber(this);
            if (this.virtualDisplacementMapType == null)
                this.virtualDisplacementMapType = new VdispMapType(this, this.Symbol);
            if (this.externalDataType == null)
                this.externalDataType = new ExternalDataType(this, this.Symbol);
        }

        protected override DecoratedName GenerateName()
        {
            this.NeedsModifiers = false;

            var declaration = new DecoratedName(this, this.Symbol);
            if (this.TypeEncoding.IsVirtualFunctionTable)
            {
                this.virtualFunctionTableType.SuperType = declaration;
                return this.virtualFunctionTableType.Name;
            }
            if (this.TypeEncoding.IsVirtualBaseTable)
            {
                this.virtualBaseTableType.SuperType = declaration;
                return this.virtualBaseTableType.Name;
            }
            if (this.TypeEncoding.IsGuard)
            {
                declaration.Append('{');
                declaration.Append(this.guardNumber.Name);
                return declaration + "}'";
            }
            if (this.TypeEncoding.IsVirtualDisplacementMap)
            {
                this.virtualDisplacementMapType.SuperType = declaration;
                return this.virtualDisplacementMapType.Name;
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
                this.externalDataType.SuperType = declaration;
                declaration.Assign(this.externalDataType.Name);
            }

            this.NeedsModifiers = true;
            return declaration;
        }

        private unsafe void Parse(ref char* pSource)
        {
            if (this.TypeEncoding.IsVirtualFunctionTable)
            {
                this.virtualFunctionTableType = new VfTableType(this, ref pSource, this.Symbol);
                return;
            }
            if (this.TypeEncoding.IsVirtualBaseTable)
            {
                this.virtualBaseTableType = new VbTableType(this, ref pSource, this.Symbol);
                return;
            }
            if (this.TypeEncoding.IsGuard)
            {
                this.guardNumber = new GuardNumber(this, ref pSource);
                return;
            }
            if (this.TypeEncoding.IsVirtualDisplacementMap)
            {
                this.virtualDisplacementMapType = new VdispMapType(this, ref pSource, this.Symbol);
                return;
            }

            if (!(this.TypeEncoding.IsThunk && this.TypeEncoding.IsDestructorHelper) &&
                !this.TypeEncoding.IsDataMemberConstructorHelper &&
                !this.TypeEncoding.IsDataMemberDestructorHelper &&
                this.TypeEncoding.IsName)
                return; // new DecoratedName(declaration);

            if (!(this.TypeEncoding.IsDataMemberConstructorHelper ||
                  this.TypeEncoding.IsDataMemberDestructorHelper))
                this.externalDataType = new ExternalDataType(this, ref pSource, this.Symbol);
        }

        protected override DecoratedName GenerateCode()
        {
            if (this.TypeEncoding.IsVirtualFunctionTable)
                return this.virtualFunctionTableType.Code;
            if (this.TypeEncoding.IsVirtualBaseTable)
                return this.virtualBaseTableType.Code;
            if (this.TypeEncoding.IsGuard)
                return this.guardNumber.Code;
            if (this.TypeEncoding.IsVirtualDisplacementMap)
                return this.virtualDisplacementMapType.Code;

            if (!(this.TypeEncoding.IsThunk && this.TypeEncoding.IsDestructorHelper) &&
                !this.TypeEncoding.IsDataMemberConstructorHelper &&
                !this.TypeEncoding.IsDataMemberDestructorHelper &&
                this.TypeEncoding.IsName)
                return new DecoratedName();

            if (!(this.TypeEncoding.IsDataMemberConstructorHelper ||
                  this.TypeEncoding.IsDataMemberDestructorHelper))
                return this.externalDataType.Code;

            return new DecoratedName();
        }
    }
}