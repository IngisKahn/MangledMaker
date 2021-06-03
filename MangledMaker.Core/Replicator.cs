namespace MangledMaker.Core
{
    using System.Diagnostics;
    using Elements;

#if TESTING
    public partial
#endif

    public class Replicator
    {
        private readonly DecoratedName errorName = new DecoratedName(NodeStatus.Error);
        private readonly DecoratedName invalidName = new DecoratedName(NodeStatus.Invalid);
        private int currentIndex = -1;
        private Element[] savedName = new Element[9];

        internal bool IsFull
        {
            get { return this.currentIndex == 8; }
        }

        internal DecoratedName this[int x]
        {
            get
            {
                if (x < 0 || x > 8)
                    return this.errorName;
                return x > this.currentIndex ? this.invalidName : this.savedName[x].Name;
            }
        }

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