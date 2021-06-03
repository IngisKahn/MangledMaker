namespace MangledMaker.Core.Elements
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Attributes;

    public abstract class Element
    {
        protected Element(Element parent)
        {
            this.Parent = parent;
        }

        public Element Parent { get; private set; }

        protected bool IsTruncated { private get; set; }

        protected bool IsInvalid { private get; set; }

        public DecoratedName Name
        {
            get
            {
                this.CreateEmptyElements();
                if (this.IsTruncated)
                    return new DecoratedName(this, NodeStatus.Truncated);
                return this.IsInvalid ? new DecoratedName(this, NodeStatus.Invalid) : this.GenerateName();
            }
        }

        public DecoratedName Code
        {
            get
            {
                this.CreateEmptyElements();
                if (this.IsTruncated || this.IsInvalid)
                    return new DecoratedName();
                return this.GenerateCode();
            }
        }

        private IEnumerable<Element> Children
        {
            get
            {
                // use reflection to get all properties that are elements
                var pia = this.GetType().GetProperties();

                foreach (var pi in pia)
                    if (pi.GetCustomAttributes(typeof(ChildAttribute), false).Length != 0)
                    {
                        var type = pi.PropertyType;
                        if (type.IsSubclassOf(typeof(Element)))
                            yield return (Element)pi.GetValue(this, null);
                        else
                        {
                            var list = pi.GetValue(this, null) as IList;
                            if (list == null) 
                                continue;
                            foreach (var e in list.OfType<Element>())
                                yield return e;
                        }
                    }
            }
        }

        protected virtual void CreateEmptyElements()
        {
        }

        protected abstract DecoratedName GenerateName();
        protected abstract DecoratedName GenerateCode();

        public override string ToString()
        {
            return this.Name.ToString();
        }

        public bool IsAncestorOf(Element test)
        {
            return this.Children.Any(child => child == test || child != null && child.IsAncestorOf(test));
        }

        protected static char IndirectionToChar(IndirectType type)
        {
            switch (type)
            {
                case IndirectType.Pointer:
                    return '*';
                case IndirectType.Reference:
                    return '&';
                default:
                    return '\0';
            }
        }
    }
}