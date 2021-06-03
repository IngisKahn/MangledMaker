namespace MangledMaker.Core
{
    using Elements;

    /// <summary>
    /// Base class for an element of linked list of text in a decorated name
    /// </summary>
    public abstract class DecoratedNameNode
    {
        /// <summary>
        /// Creates a new instance of DecoratedNameNode
        /// </summary>
        /// <param name="parent">The name element to be represented</param>
        protected DecoratedNameNode(Element parent)
        {
            this.Parent = parent;
        }

        // Gets the name element that this node represents
        public Element Parent { get; private set; }
        
        /// <summary>
        /// Gets the next decorated name node in the list
        /// </summary>
        public DecoratedNameNode NextNode { get; private set; }

        /// <summary>
        /// Gets the number of characters in this node
        /// </summary>
        public abstract int Length { get; }

        /// <summary>
        /// Gets the last character in this node.
        /// </summary>
        public abstract char LastCharacter { get; }

        /// <summary>
        /// Create a reference to this node
        /// </summary>
        /// <returns>A reference to the list of nodes starting with the current</returns>
        public DecoratedNameNode Clone()
        {
            return new NameReferenceNode(this.Parent, new DecoratedName(this));
        }

        /// <summary>
        /// Appends a node onto the end of the list of nodes
        /// </summary>
        /// <param name="node">The node to append</param>
        public void Append(DecoratedNameNode node)
        {
            if (node == null) 
                return;
            if (this.NextNode != null)
            {
                var midNode = this.NextNode;
                while (midNode.NextNode != null)
                    midNode = midNode.NextNode;
                midNode.NextNode = node;
            }
            else
                this.NextNode = node;
        }
    }
}