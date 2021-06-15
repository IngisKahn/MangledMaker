namespace MangledMaker.Core.Elements
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Attributes;

    public abstract class Element
    {
        protected Element(Element? parent) => this.Parent = parent;

        public Element? Parent { get; }

        protected bool IsTruncated { private get; set; }

        protected bool IsInvalid { private get; set; }

        public DecoratedName Name =>
            this.IsTruncated ? new(this, NodeStatus.Truncated) :
            this.IsInvalid ? new(this, NodeStatus.Invalid) : this.GenerateName();

        public DecoratedName Code => this.IsTruncated || this.IsInvalid ? new() : this.GenerateCode();

        private IEnumerable<Element?> Children
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
                            yield return (Element?)pi.GetValue(this, null);
                        else
                        {
                            if (pi.GetValue(this, null) is not IList list)
                                continue;
                            foreach (var e in list.OfType<Element>())
                                yield return e;
                        }
                    }
            }
        }

        protected abstract DecoratedName GenerateName();
        protected abstract DecoratedName GenerateCode();

        public override string ToString() => this.Name.ToString();

        public bool IsAncestorOf(Element test) => this.Children.Any(child => child == test || child != null && child.IsAncestorOf(test));

        protected static char IndirectionToChar(IndirectType type) =>
            type switch
            {
                IndirectType.Pointer => '*',
                IndirectType.Reference => '&',
                _ => '\0'
            };
    }
}