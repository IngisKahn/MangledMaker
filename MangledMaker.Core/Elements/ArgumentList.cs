namespace MangledMaker.Core.Elements
{
    using System.Collections.Generic;

    using Attributes;

    public sealed class ArgumentList : ComplexElement, ISpawnsChildren
    {
        public ArgumentList(ComplexElement parent)
            : base(parent)
        {
            this.Arguments = new List<Argument>();
        }

        public unsafe ArgumentList(ComplexElement parent, ref char* pSource)
            : base(parent)
        {
            this.Arguments = new List<Argument>();
            this.Parse(ref pSource);
        }

        [Child]
        public List<Argument> Arguments { get; set; }

        public Element CreateChild()
        {
            return new Argument(this);
        }

        protected override DecoratedName GenerateName()
        {
            var first = true;
            var aList = new DecoratedName(this);

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
            var aList = new DecoratedName();
            this.Arguments.Clear();
            while (aList.Status == NodeStatus.None
                   && *pSource != 'Z'
                   && *pSource != '@')
                if (*pSource != '\0')
                {
                    var arg = new Argument(this, ref pSource);
                    this.Arguments.Add(arg);
                    aList.Skip(arg.Name);
                }
                else
                    //aList.Append(NodeStatus.Missing);
                    break;
        }

        protected override DecoratedName GenerateCode()
        {
            var aList = new DecoratedName();

            foreach (var arg in this.Arguments)
                aList.Append(arg.Code);
            return aList;
        }
    }
}