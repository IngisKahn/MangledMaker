namespace MangledMaker.Core.Elements
{
    using System.Collections.Generic;
    using Attributes;

    public sealed class TemplateArgumentList : ComplexElement, ISpawnsChildren
    {
        public TemplateArgumentList(ComplexElement parent)
            : base(parent)
        {
            this.Arguments = new List<TemplateArgument>();
        }

        public unsafe TemplateArgumentList(ComplexElement parent, ref char* pSource)
            : base(parent)
        {
            this.Arguments = new List<TemplateArgument>();
            this.Parse(ref pSource);
        }

        [Child]
        public List<TemplateArgument> Arguments { get; set; }

        public Element CreateChild()
        {
            return new TemplateArgument(this);
        }

        protected override DecoratedName GenerateName()
        {
            var first = true;
            var aList = new DecoratedName();
            this.UnDecorator.CanGetTemplateArgumentList = true;

            foreach (var arg in this.Arguments)
            {
                if (first)
                    first = false;
                else
                    aList.Append(',');
                aList.Append(arg.Name);
            }

            this.UnDecorator.CanGetTemplateArgumentList = false;

            return aList;
        }

        private unsafe void Parse(ref char* pSource)
        {
            var aList = new DecoratedName();
            this.Arguments.Clear();
            while (aList.Status == NodeStatus.None
                   && *pSource != '\0'
                   && *pSource != '@')
            {
                var arg = new TemplateArgument(this, ref pSource);
                this.Arguments.Add(arg);
                aList.Skip(arg.Name);
            }
        }

        protected override DecoratedName GenerateCode()
        {
            //bool first = true;
            var aList = new DecoratedName();
            this.UnDecorator.CanGetTemplateArgumentList = true;

            foreach (var arg in this.Arguments)
            {
                //if (first)
                //    first = false;
                //else
                //    aList.Append(',');
                aList.Append(arg.Code);
            }

            this.UnDecorator.CanGetTemplateArgumentList = false;

            return aList;
        }
    }
}