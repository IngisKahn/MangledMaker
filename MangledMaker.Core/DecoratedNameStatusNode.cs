namespace MangledMaker.Core
{
    using Elements;

    internal class DecoratedNameStatusNode : DecoratedNameNode
    {
        private readonly NodeStatus status;

/*
        public DecoratedNameStatusNode(NodeStatus status) : this(null, status)
        {
        }
*/

        public DecoratedNameStatusNode(Element? parent, NodeStatus status)
            : base(parent)
        {
            this.status = status;
            this.Length = status == NodeStatus.Truncated ? 4 : 0;
        }

        public override int Length { get; }

        public override char LastCharacter => this.status == NodeStatus.Truncated ? ' ' : '\0';

        public override string ToString() => this.status == NodeStatus.Truncated ? " ?? " : string.Empty;
    }
}