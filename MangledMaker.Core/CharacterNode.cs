namespace MangledMaker.Core
{
    using System.Globalization;
    using Elements;

    /// <summary>
    /// Represents a single character of text in a decorated name
    /// </summary>
    internal class CharacterNode : DecoratedNameNode
    {
        private readonly char character;

        //public CharacterNode(char character) : this(null, character) { }
        public CharacterNode(Element? parent, char character)
            : base(parent) =>
            this.character = character;

        public override char LastCharacter => this.character;

        public override int Length => 1;

        public override string ToString() => this.character.ToString(CultureInfo.InvariantCulture);
    }
}