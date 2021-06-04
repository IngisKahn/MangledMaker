namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class ArgumentTypes : ComplexElement
    {
        public enum ArgumentMode
        {
            Void,
            Ellipsis,
            List,
            ListEllipsis
        }

        public ArgumentTypes(ComplexElement parent)
            : base(parent)
        {
        }

        public unsafe ArgumentTypes(ComplexElement parent, ref char* pSource)
            : base(parent) =>
            this.Parse(ref pSource);

        [Setting]
        public ArgumentMode Mode { get; set; }

        private ArgumentList? argumentList;

        [Child]
        public ArgumentList ArgumentList
        {
            get => this.argumentList ??= new(this);
            private set => this.argumentList = value;
        }

        protected override DecoratedName GenerateName()
        {
            switch (this.Mode)
            {
                case ArgumentMode.Void:
                    return new(this, "void");
                case ArgumentMode.Ellipsis:
                    return new(this, this.UnDecorator.DoEllipsis ? "..." : "<ellipsis>");
                default:
                    var list = this.ArgumentList.Name;
                    if (list.Status != 0) 
                        return new(list);
                    switch (this.Mode)
                    {
                        case ArgumentMode.ListEllipsis:
                            return list +
                                   new DecoratedName(this, this.UnDecorator.DoEllipsis ? ",..." : ",<ellipsis>");
                        default:
                            return list;
                    }
            }
        }

        private unsafe void Parse(ref char* pSource)
        {
            switch (*pSource)
            {
                case 'X':
                    pSource++;
                    this.Mode = ArgumentMode.Void;
                    break;
                case 'Z':
                    pSource++;
                    this.Mode = ArgumentMode.Ellipsis;
                    return;
                default:
                    this.ArgumentList = new(this, ref pSource);
                    if (this.ArgumentList.Name.Status == NodeStatus.None)
                        switch (*pSource)
                        {
                            case '\0':
                                this.Mode = ArgumentMode.List;
                                break;
                            case '@':
                                pSource++;
                                this.Mode = ArgumentMode.List;
                                break;
                            case 'Z':
                                this.Mode = ArgumentMode.ListEllipsis;
                                break;
                            default:
                                this.IsInvalid = true;
                                break;
                        }
                    break;
            }
        }

        protected override DecoratedName GenerateCode()
        {
            DecoratedName code = new(this);
            switch (this.Mode)
            {
                case ArgumentMode.Void:
                    code.Assign('X');
                    break;
                case ArgumentMode.Ellipsis:
                    code.Assign('Z');
                    break;
                case ArgumentMode.List:
                    code.Assign(this.ArgumentList.Code);
                    code.Append('@');
                    break;
                case ArgumentMode.ListEllipsis:
                    code.Assign(this.ArgumentList.Code);
                    code.Append('Z');
                    break;
            }
            return code;
        }
    }
}