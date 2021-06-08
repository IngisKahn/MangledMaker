namespace MangledMaker.Core
{
    using System.Diagnostics;
    using Elements;

#if TESTING
    public partial
#endif

    public class Replicator
    {
        private readonly DecoratedName errorName = new(NodeStatus.Error);
        private readonly DecoratedName invalidName = new(NodeStatus.Invalid);
        private int currentIndex = -1;
        private Element[] savedName = new Element[9];

        internal bool IsFull => this.currentIndex == 8;

        internal DecoratedName this[int x] =>
            x is < 0 or > 8 ? this.errorName :
            x > this.currentIndex ? this.invalidName : this.savedName[x].Name;

        public void Append(Element rd)
        {
            Debug.WriteLine(rd.ToString());
            if (!this.IsFull && !rd.Name.IsEmpty)
                this.savedName[++this.currentIndex] = rd;
        }

        public virtual void Reset()
        {
            this.currentIndex = -1;
            this.savedName = new Element[9];
        }
    }
}