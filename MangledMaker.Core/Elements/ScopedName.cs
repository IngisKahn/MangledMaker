namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class ScopedName : ComplexElement
    {
        public ScopedName(ComplexElement parent)
            : base(parent)
        { }

        public unsafe ScopedName(ComplexElement parent, ref char* pSource)
            : base(parent)
        {
            this.Parse(ref pSource);
        }

        [Child]
        public ZName ZName { get; set; }

        [Child]
        public Scope Scope { get; set; }

        protected override void CreateEmptyElements()
        {
            if (this.ZName == null) this.ZName = new ZName(this, true);
            if (this.Scope == null) this.Scope = new Scope(this);
        }

        protected override DecoratedName GenerateName()
        {
            var name = new DecoratedName(this, this.ZName.Name);
            if (name.Status != NodeStatus.None || this.Scope.Name.Length == 0) 
                return name;
            name.Prepend("::");
            name.Prepend(this.Scope.Name);

            return name;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.ZName = new ZName(this, ref pSource, true);
            var name = this.ZName.Name;
            //name.Assign();
            var cur = *pSource;
            if (name.Status == NodeStatus.None
                && cur != '\0'
                && cur != '@')
            {
                this.Scope = new Scope(this, ref pSource);
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
            var code = new DecoratedName(this, this.ZName.Code);
            if (!this.Scope.Code.IsEmpty)
                code += this.Scope.Code;
            code += '@';
            return code;
        }
    }
}