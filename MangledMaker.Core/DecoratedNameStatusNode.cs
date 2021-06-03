namespace MangledMaker.Core
{
    using Elements;

    internal class DecoratedNameStatusNode : DecoratedNameNode
    {
        private readonly int length; // member C
        private readonly NodeStatus status;

/*
        public DecoratedNameStatusNode(NodeStatus status) : this(null, status)
        {
        }
*/

        public DecoratedNameStatusNode(Element parent, NodeStatus status)
            : base(parent)
        {
            this.status = status;
            this.length = status == NodeStatus.Truncated ? 4 : 0;
        }

        public override int Length
        {
            get { return this.length; }
        }

        public override char LastCharacter
        {
            get { return this.status == NodeStatus.Truncated ? ' ' : '\0'; }
        }

        public override string ToString()
        {
            return this.status == NodeStatus.Truncated ? " ?? " : string.Empty;
        }
    }
}