namespace MangledMaker.Core
{
    using Elements;

    internal sealed class StringNode : DecoratedNameNode
    {
        private readonly string stringValue;

/*
        public StringNode(string str) : this(null, str)
        {
        }
*/

        public StringNode(Element? parent, string str)
            : base(parent) =>
            this.stringValue = str;

        public override char LastCharacter => this.stringValue[^1];

        public override int Length => this.stringValue.Length;

        public override string ToString() => this.stringValue;
    }
}