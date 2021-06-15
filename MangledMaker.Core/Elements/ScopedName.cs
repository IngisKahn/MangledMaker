namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class ScopedName : ComplexElement
    {
        public ScopedName(ComplexElement parent)
            : base(parent)
        { }

        public unsafe ScopedName(ComplexElement parent, ref char* pSource)
            : base(parent) =>
            this.Parse(ref pSource);

        private ZName? zName;
        [Child]
        public ZName ZName { get => this.zName ??= new(this, true); set => this.zName = value; }

        private Scope? scope;
        [Child]
        public Scope Scope { get => this.scope ??= new(this); set => this.scope = value; }

        protected override DecoratedName GenerateName()
        {
            DecoratedName name = new(this, this.ZName.Name);
            if (name.Status != NodeStatus.None || this.Scope.Name.Length == 0) 
                return name;
            name.Prepend("::");
            name.Prepend(this.Scope.Name);

            return name;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.ZName = new(this, ref pSource, true);
            var name = this.ZName.Name;
            //name.Assign();
            var cur = *pSource;
            if (name.Status == NodeStatus.None
                && cur != '\0'
                && cur != '@')
            {
                this.Scope = new(this, ref pSource);
                name.Append(this.Scope.Name);
                cur = *pSource;
            }
            if (cur == '@')
                pSource++;
            else if (name.IsEmpty)
                this.IsTruncated = true;
        }

        protected override DecoratedName GenerateCode()
        {
            DecoratedName code = new(this, this.ZName.Code);
            if (!this.Scope.Code.IsEmpty)
                code += this.Scope.Code;
            code += '@';
            return code;
        }
    }
}