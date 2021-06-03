namespace MangledMaker.Core.Elements
{
    using System.Collections.Generic;
    using Attributes;

    public sealed class DataIndirectType : ComplexElement, ISpawnsChildren
    {
        private int completionLevel;
        private bool isBased;
        private bool isConst;
        private bool isNamed;
        private bool isVolatile;

        public DataIndirectType(ComplexElement parent)
            : this(parent, new DecoratedName(), (IndirectType)'\0',
                   new DecoratedName(), false)
        {
        }

        public DataIndirectType(ComplexElement parent, DecoratedName superType,
                                IndirectType prType, DecoratedName cvType, bool thisFlag)
            : base(parent)
        {
            this.SpecialModifiers = new List<DataSpecialModifier>();
            this.SuperType = superType;
            this.PrType = prType;
            this.CvType = cvType;
            this.IsThis = thisFlag;
        }

        public unsafe DataIndirectType(ComplexElement parent, ref char* pSource,
                                       DecoratedName superType, IndirectType prType, DecoratedName cvType, bool thisFlag)
            : this(parent, superType, prType, cvType, thisFlag)
        {
            this.Parse(ref pSource);
        }

        public unsafe DataIndirectType(ComplexElement parent, ref char* pSource)
            : this(parent)
        {
            this.Parse(ref pSource);
        }

        [Input]
        public DecoratedName SuperType { get; set; }

        [Input]
        public IndirectType PrType { get; set; }

        [Input]
        public DecoratedName CvType { get; set; }

        [Input]
        public bool IsThis { get; set; }

        [Setting]
        public bool IsNamed
        {
            get { return this.isNamed; }
            set
            {
                this.completionLevel = int.MaxValue;
                this.isNamed = value;
            }
        }

        [Setting]
        public bool IsConst
        {
            get { return this.isConst; }
            set
            {
                this.completionLevel = int.MaxValue;
                this.isConst = value;
                if (value)
                    this.isBased = false;
            }
        }

        [Setting]
        public bool IsVolatile
        {
            get { return this.isVolatile; }
            set
            {
                this.completionLevel = int.MaxValue;
                this.isVolatile = value;
                if (value)
                    this.isBased = false;
            }
        }

        [Setting]
        public bool IsBased
        {
            get { return this.isBased; }
            set
            {
                this.completionLevel = int.MaxValue;
                this.isBased = value;
                if (value)
                    this.isConst = this.isVolatile = false;
            }
        }

        [Child]
        public List<DataSpecialModifier> SpecialModifiers { get; private set; }

        [Child]
        public Scope DataName { get; private set; }

        [Child]
        public BasedType BasedType { get; private set; }

        public Element CreateChild()
        {
            return new DataSpecialModifier(this, this.PrType, this.IsThis, new DecoratedName(),
                                           new DecoratedName());
        }

        protected override void CreateEmptyElements()
        {
            if (this.DataName == null)
                this.DataName = new Scope(this);
            if (this.BasedType == null)
                this.BasedType = new BasedType(this);
        }

        protected override DecoratedName GenerateName()
        {
            var bIsPinPtr = false;
            var dataChar = IndirectionToChar(this.PrType);
            if (this.completionLevel <= 0)
            {
                if (!this.IsThis && !this.SuperType.IsEmpty)
                {
                    if (this.CvType.IsEmpty)
                        return new DecoratedName(this, NodeStatus.Truncated) + this.SuperType;
                    var m = new DecoratedName(this, NodeStatus.Truncated);
                    m.Append(this.CvType);
                    m.Append(' ');
                    return m + this.SuperType;
                }
                if (!this.IsThis && !this.CvType.IsEmpty)
                    return new DecoratedName(this, NodeStatus.Truncated) + this.CvType;
                return new DecoratedName(this, NodeStatus.Truncated);
            }

            var keywords = new DecoratedName(this);
            var unaligned = new DecoratedName(this);
            foreach (var dsm in this.SpecialModifiers)
            {
                dsm.Indirection = this.PrType;
                dsm.IsThis = this.IsThis;
                dsm.Keywords = keywords;
                dsm.Unaligned = unaligned;
                // this populates keywords and unaligned
                var extended = dsm.Name;

                if (!dsm.IsExtended)
                    continue;
                if (!extended.IsEmpty)
                    return extended;
                dataChar = dsm.Prefix;
                bIsPinPtr |= dsm.ExtendedDataIndirectType.Mode == IndirectMode.Pin;
            }


            var dataType = new DecoratedName(this, dataChar);
            if (!keywords.IsEmpty)
                dataType.Append(' ' + keywords);

            if (!unaligned.IsEmpty)
                dataType.Prepend(unaligned + ' ');

            if (this.isNamed) // = Q0123
            {
                if (dataChar != '\0')
                {
                    dataType.Prepend("::");
                    if (this.completionLevel > 1)
                        dataType.Prepend(this.DataName.Name);
                    else
                        dataType.Prepend(NodeStatus.Truncated);
                }
                else if (this.completionLevel > 1)
                    dataType.Skip(this.DataName.Name);
                if (this.completionLevel <= 2)
                    dataType.Append(NodeStatus.Truncated);
            }
            if (this.isBased)
                if (this.UnDecorator.DoMicrosoftKeywords)
                    dataType.Prepend(this.BasedType.Name);
                else
                    dataType.Skip(this.BasedType.Name);
            if (this.isVolatile)
                dataType.Prepend("volatile ");
            if (this.isConst)
                dataType.Prepend("const ");
            if (!this.IsThis)
                if (!this.SuperType.IsEmpty)
                    if (this.SuperType.IsPointerReference || this.CvType.IsEmpty)
                        if (this.SuperType.IsArray)
                            dataType.Assign(this.SuperType);
                        else
                        {
                            dataType.Append(' ');
                            dataType.Append(this.SuperType);
                        }
                    else
                    {
                        dataType.Append(' ');
                        dataType.Append(this.CvType);
                        dataType.Append(' ');
                        dataType.Append(this.SuperType);
                    }
                else if (!this.CvType.IsEmpty)
                {
                    dataType.Append(' ');
                    dataType.Append(this.CvType);
                }
            dataType.IsPointerReference = true;
            if (bIsPinPtr)
                dataType.IsPinnedPointer = true;
            return dataType;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.SpecialModifiers.Clear();
            this.completionLevel = 0;

            if (*pSource == '\0')
                return;
            this.completionLevel++;
            var keywords = new DecoratedName(this);
            var unaligned = new DecoratedName(this);

            var hasSpecialPrefix = true;
            do
                switch (*pSource)
                {
                    case '$':
                    case 'E':
                    case 'F':
                    case 'I':
                        var special = new DataSpecialModifier(this, ref pSource,
                            this.PrType, this.IsThis, keywords, unaligned);
                        this.SpecialModifiers.Add(special);
                        if (!special.Name.IsEmpty)
                            return;
                        pSource++;
                        break;
                    default:
                        hasSpecialPrefix = false;
                        break;
                } while (hasSpecialPrefix);


            var charOffset = *pSource - (*pSource < 0x41 ? 0x16 : 0x41);
            pSource++;
            if (charOffset <= 0x1f) // 0-5 A-`
            {
                if ((charOffset & 0x10) != 0) // = Q0123
                {
                    this.isNamed = true;
                    if (this.IsThis)
                    {
                        this.IsInvalid = true;
                        return;
                    }

                    if (this.PrType != IndirectType.Default)
                        if (*pSource != '\0')
                        {
                            this.completionLevel++;
                            this.DataName = new Scope(this, ref pSource);
                        }
                        else
                            return;
                    else if (*pSource != '\0')
                        this.DataName = new Scope(this, ref pSource);

                    if (*pSource == '\0')
                        return;
                    this.completionLevel++;
                    if (*pSource++ != '@')
                        this.IsInvalid = true;
                    return;
                }
                this.isNamed = false;

                if ((charOffset & 0xC) == 0xC) //2M
                {
                    this.isBased = true;
                    this.BasedType = new BasedType(this, ref pSource);
                }
                else
                    this.isBased = false;

                this.isVolatile = (charOffset & 2) != 0; //CD01

                this.isConst = (charOffset & 1) != 0; //13BD

                return;
            }

            this.IsInvalid = true;
        }

        protected override DecoratedName GenerateCode()
        {
            var code = new DecoratedName(this);
            if (this.completionLevel <= 0)
                return new DecoratedName(this, '\0');
            foreach (var dsm in this.SpecialModifiers)
            {
                dsm.Indirection = this.PrType;
                dsm.IsThis = this.IsThis;
                code.Append(dsm.Code);
                if (!dsm.Name.IsEmpty)
                    return code;
            }

            char c;
            if (this.isBased)
                c = this.isNamed ? '2' : 'M';
            else if (this.isConst)
                if (this.isVolatile)
                    c = this.isNamed ? '1' : 'D';
                else
                    c = this.isNamed ? '3' : 'B';
            else if (this.isVolatile)
                c = this.isNamed ? '0' : 'C';
            else
                c = this.isNamed ? 'Q' : 'A';
            code.Append(c);

            if (this.isNamed)
                if (this.completionLevel > 1)
                {
                    code.Append(this.DataName.Code);
                    if (this.completionLevel == 2)
                        code.Append('\0');
                }
                else
                    code.Append('\0');

            if (this.isBased)
                code.Append(this.BasedType.Code);

            return code;
        }
    }
}