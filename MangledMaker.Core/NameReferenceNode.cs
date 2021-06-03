namespace MangledMaker.Core
{
    using Elements;

    internal sealed class NameReferenceNode : DecoratedNameNode
    {
/*
        public NameReferenceNode(DecoratedName name) : this(null, name)
        {
        }
*/

        public NameReferenceNode(Element parent, DecoratedName name)
            : base(parent)
        {
            if ((name != null) &&
                (name.Status != NodeStatus.Truncated) &&
                (name.Status != NodeStatus.Error))
                this.Reference = name;
        }

        public DecoratedName Reference { get; private set; }

        public override char LastCharacter
        {
            get { return this.Reference == null ? '\0' : this.Reference.LastCharacter; }
        }

        public override int Length
        {
            get { return this.Reference == null ? 0 : this.Reference.Length; }
        }

        public override string ToString()
        {
            return this.Reference == null ? string.Empty : this.Reference.ToString();
        }
    }
}