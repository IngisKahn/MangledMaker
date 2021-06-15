namespace MangledMaker.Core.Elements
{
    using System;
    using System.Collections.Generic;
    using Attributes;

    public sealed class VfTableType : ComplexElement, ISpawnsChildren
    {
        private bool isMissingList;

        private bool isMissingTerminator;

        public VfTableType(ComplexElement parent, DecoratedName superType)
            : base(parent)
        {
            this.OwnerHierarchy = new();
            this.SuperType = superType;
            this.StorageConvention = new(this);
        }

        public unsafe VfTableType(ComplexElement parent, ref char* pSource,
                                  DecoratedName superType)
            : base(parent)
        {
            this.OwnerHierarchy = new();
            this.SuperType = superType;
            this.Parse(ref pSource);
            this.StorageConvention ??= new(this);
        }

        [Input]
        public DecoratedName SuperType { get; set; }

        [Child]
        public List<Scope> OwnerHierarchy { get; private set; }

        [Child]
        public StorageConvention StorageConvention { get; private set; }

        #region ISpawnsChildren Members

        public Element CreateChild() => new Scope(this);

        #endregion

        protected override DecoratedName GenerateName()
        {
            var vxTableName = new DecoratedName(this, this.SuperType);
            switch (vxTableName.IsValid)
            {
                case true when !this.isMissingList:
                {
                    vxTableName.Assign(this.StorageConvention.Name + (' ' + vxTableName));
                    if (!vxTableName.IsValid || this.OwnerHierarchy.Count == 0) 
                        return vxTableName;
                    vxTableName += "for{ ";
                    for (var i = 0; i < this.OwnerHierarchy.Count; i++)
                    {
                        vxTableName += '`';
                        vxTableName += this.OwnerHierarchy[i].Name;
                        vxTableName += '\'';
                        if (i != this.OwnerHierarchy.Count - 1)
                            vxTableName += "s ";
                    }
                    if (!vxTableName.IsValid) 
                        return vxTableName;
                    if (this.isMissingTerminator)
                        vxTableName += NodeStatus.Truncated;
                    vxTableName += '}';
                    break;
                }
                case true:
                    vxTableName.Assign(NodeStatus.Truncated + vxTableName);
                    break;
            }
            return vxTableName;
        }

        private unsafe void Parse(ref char* pSource)
        {
            if (this.SuperType.IsValid && (*pSource != '\0'))
            {
                this.isMissingList = false;
                this.StorageConvention = new(this, ref pSource);
                if (!this.StorageConvention.Name.IsValid) 
                    return;
                if (*pSource != '@')
                {
                    this.OwnerHierarchy.Clear();
                    DecoratedName status = new();
                    while (status.IsValid &&
                           *pSource != '\0' &&
                           *pSource != '@')
                    {
                        Scope s = new(this, ref pSource);
                        status = s.Name;
                        this.OwnerHierarchy.Add(s);
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
            DecoratedName vxTableCode = new(this);
            switch (this.SuperType.IsValid)
            {
                case true when !this.isMissingList:
                {
                    vxTableCode += this.StorageConvention.Code;
                    if (!vxTableCode.IsValid) 
                        return vxTableCode;
                    if (this.OwnerHierarchy.Count != 0)
                    {
                        vxTableCode += "for{ ";
                        foreach (var scope in this.OwnerHierarchy)
                        {
                            vxTableCode += scope.Code;
                            vxTableCode += '@';
                        }
                    }
                    vxTableCode += this.isMissingTerminator ? '\0' : '@';
                    break;
                }
                case true:
                    vxTableCode += '\0';
                    break;
            }
            return vxTableCode;
        }
    }
}