namespace MangledMaker.Core.Elements
{
    using System.Collections.Generic;

    using Attributes;

    public sealed class ArgumentList : ComplexElement, ISpawnsChildren
    {
        public ArgumentList(ComplexElement parent)
            : base(parent)
        { }

        public unsafe ArgumentList(ComplexElement parent, ref char* pSource)
            : base(parent) =>
            this.Parse(ref pSource);

        [Child] public List<Argument> Arguments { get; } = new();

        public Element CreateChild() => new Argument(this);

        protected override DecoratedName GenerateName()
        {
            var first = true;
            DecoratedName aList = new(this);

            foreach (var arg in this.Arguments)
            {
                if (first)
                    first = false;
                else
                    aList.Append(',');
                aList.Append(new DecoratedName(this, arg.Name.ToString()));
            }
            return aList;
        }

        private unsafe void Parse(ref char* pSource)
        {
            DecoratedName aList = new();
            this.Arguments.Clear();
            while (aList.Status == NodeStatus.None
                   && *pSource != 'Z'
                   && *pSource != '@')
                if (*pSource != '\0')
                {
                    Argument arg = new(this, ref pSource);
                    this.Arguments.Add(arg);
                    aList.Skip(arg.Name);
                }
                else
                    //aList.Append(NodeStatus.Missing);
                    break;
        }

        protected override DecoratedName GenerateCode()
        {
            DecoratedName aList = new();

            foreach (var arg in this.Arguments)
                aList.Append(arg.Code);
            return aList;
        }
    }
}