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

        public NameReferenceNode(Element? parent, DecoratedName? name)
            : base(parent)
        {
            if ((name != null) &&
                (name.Status != NodeStatus.Truncated) &&
                (name.Status != NodeStatus.Error))
                this.Reference = name;
        }

        public DecoratedName? Reference { get; }

        public override char LastCharacter => this.Reference?.LastCharacter ?? '\0';

        public override int Length => this.Reference?.Length ?? 0;

        public override string ToString() => this.Reference == null ? string.Empty : this.Reference.ToString();
    }
}