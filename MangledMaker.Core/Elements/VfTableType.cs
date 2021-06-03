namespace MangledMaker.Core.Elements
{
    using System.Collections.Generic;
    using Attributes;

    public sealed class VfTableType : ComplexElement, ISpawnsChildren
    {
        private bool isMissingList;

        private bool isMissingTerminator;

        public VfTableType(ComplexElement parent, DecoratedName superType)
            : base(parent)
        {
            this.OwnerHeirarchy = new List<Scope>();
            this.SuperType = superType;
            this.StorageConvention = new StorageConvention(this);
        }

        public unsafe VfTableType(ComplexElement parent, ref char* pSource,
                                  DecoratedName superType)
            : base(parent)
        {
            this.OwnerHeirarchy = new List<Scope>();
            this.SuperType = superType;
            this.Parse(ref pSource);
        }

        [Input]
        public DecoratedName SuperType { get; set; }

        [Child]
        public List<Scope> OwnerHeirarchy { get; private set; }

        [Child]
        public StorageConvention StorageConvention { get; private set; }

        #region ISpawnsChildren Members

        public Element CreateChild()
        {
            return new Scope(this);
        }

        #endregion

        protected override DecoratedName GenerateName()
        {
            var vxTableName = new DecoratedName(this, this.SuperType);
            if (vxTableName.IsValid && !this.isMissingList)
            {
                vxTableName.Assign(this.StorageConvention.Name + (' ' + vxTableName));
                if (!vxTableName.IsValid || this.OwnerHeirarchy.Count == 0) 
                    return vxTableName;
                vxTableName += "for{ ";
                for (var i = 0; i < this.OwnerHeirarchy.Count; i++)
                {
                    vxTableName += '`';
                    vxTableName += this.OwnerHeirarchy[i].Name;
                    vxTableName += '\'';
                    if (i != this.OwnerHeirarchy.Count - 1)
                        vxTableName += "s ";
                }
                if (!vxTableName.IsValid) 
                    return vxTableName;
                if (this.isMissingTerminator)
                    vxTableName += NodeStatus.Truncated;
                vxTableName += '}';
            }
            else if (vxTableName.IsValid)
                vxTableName.Assign(NodeStatus.Truncated + vxTableName);
            return vxTableName;
        }

        private unsafe void Parse(ref char* pSource)
        {
            if (this.SuperType.IsValid && (*pSource != '\0'))
            {
                this.isMissingList = false;
                this.StorageConvention = new StorageConvention(this, ref pSource);
                if (!this.StorageConvention.Name.IsValid) 
                    return;
                if (*pSource != '@')
                {
                    this.OwnerHeirarchy.Clear();
                    var status = new DecoratedName();
                    while (status.IsValid &&
                           *pSource != '\0' &&
                           *pSource != '@')
                    {
                        var s = new Scope(this, ref pSource);
                        status = s.Name;
                        this.OwnerHeirarchy.Add(s);
                        if (*pSource == '@')
                            pSource++;
                    }
                    if (status.IsValid)
                        this.isMissingTerminator = *pSource == '\0';
                }
                if (*pSource == '@')
                    pSource++;
            }
            else if (this.SuperType.IsValid)
                this.isMissingList = true;
        }

        protected override DecoratedName GenerateCode()
        {
            var vxTableCode = new DecoratedName(this);
            if (this.SuperType.IsValid && !this.isMissingList)
            {
                vxTableCode += this.StorageConvention.Code;
                if (!vxTableCode.IsValid) 
                    return vxTableCode;
                if (this.OwnerHeirarchy.Count != 0)
                {
                    vxTableCode += "for{ ";
                    foreach (var scope in this.OwnerHeirarchy)
                    {
                        vxTableCode += scope.Code;
                        vxTableCode += '@';
                    }
                }
                vxTableCode += this.isMissingTerminator ? '\0' : '@';
            }
            else if (this.SuperType.IsValid)
                vxTableCode += '\0';
            return vxTableCode;
        }
    }
}