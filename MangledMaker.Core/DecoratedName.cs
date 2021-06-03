namespace MangledMaker.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using Elements;

    public class DecoratedName : IEnumerable<DecoratedNameNode>
    {
        private DecoratedNameNode node;
        private NodeStatus status;

        internal DecoratedName()
        {
        }

        internal DecoratedName(Element parent)
        {
            this.Parent = parent;
        }

        internal DecoratedName(char c)
            : this(null, c)
        {
        }

        internal DecoratedName(Element parent, char c)
            : this(parent)
        {
            if (c != '\0')
                this.DoString(c.ToString(CultureInfo.InvariantCulture));
        }

        internal DecoratedName(NodeStatus st)
            : this(null, st)
        {
        }

        internal DecoratedName(Element parent, NodeStatus st)
            : this(parent)
        {
            var vC = (st | NodeStatus.Truncated) == NodeStatus.Error
                                ? st
                                : NodeStatus.None; // if st = 1 or st = 3 then vC = st
            this.status = (vC & NodeStatus.BasicStatus)
                          | (this.status & NodeStatus.ExtendedStatus);
            this.node = new DecoratedNameStatusNode(parent, st);
            this.status &= ~NodeStatus.SpecialAndExtendedStatus;
        }

        internal DecoratedName(DecoratedName rd)
            : this(null, rd)
        {
        }

        internal DecoratedName(Element parent, DecoratedName rd)
            : this(parent)
        {
            this.node = rd.node;
            this.status = rd.status;
        }

        internal DecoratedName(DecoratedNameNode pd)
            : this(null, pd)
        {
        }

        private DecoratedName(Element parent, DecoratedNameNode pd)
            : this(parent)
        {
            this.node = pd;
        }

        /*
                public DecoratedName(string s) : this(null, s)
                {
                }
        */

        internal DecoratedName(Element parent, string s)
            : this(parent)
        {
            if (s != null) this.DoString(s);
        }

        /*
                public DecoratedName(string name, char terminator, bool doNoIdentCharCheck)
                    : this(null, name, terminator, doNoIdentCharCheck)
                {
                }
        */

        internal DecoratedName(Element parent, string name, char terminator,
                             bool doNoIdentCharCheck)
            : this(parent)
        {
            if (string.IsNullOrEmpty(name))
            {
                this.status = NodeStatus.Invalid;
                return;
            }

            if (name[name.Length - 1] != '\0')
                name = name + '\0';
            if (name[0] == '\0')
            {
                this.status = NodeStatus.Truncated;
                return;
            }

            var x = 0;
            for (; (name[x] != '\0') && (name[x] != terminator); x++)
            {
                if (IsCharValid(name[x]) || doNoIdentCharCheck)
                    continue;
                this.status = NodeStatus.Invalid;
                return;
            }
            //BCD6
            this.DoString(name.Substring(0, x));

            //TODO: FINISH
        }

        private static bool IsCharValid(char c)
        {
            switch (c)
            {
                case '_': case '$': case '<': case '>': case '-': case 'A': case 'B': case 'C': case 'D': case 'E': case 'F':
                case 'G': case 'H': case 'I': case 'J': case 'K': case 'L': case 'M': case 'N': case 'O': case 'P': case 'Q':
                case 'R': case 'S': case 'T': case 'U': case 'V': case 'W': case 'X': case 'Y': case 'Z': case 'a': case 'b':
                case 'c': case 'd': case 'e': case 'f': case 'g': case 'h': case 'i': case 'j': case 'k': case 'l': case 'm':
                case 'n': case 'o': case 'p': case 'q': case 'r': case 's': case 't': case 'u': case 'v': case 'w': case 'x':
                case 'y': case 'z': case '0': case '1': case '2': case '3': case '4': case '5': case '6': case '7': case '8':
                case '9': case '\x80': case '\x81': case '\x82': case '\x83': case '\x84': case '\x85': case '\x86': case '\x87':
                case '\x88': case '\x89': case '\x8A': case '\x8B': case '\x8C': case '\x8D': case '\x8E': case '\x8F': case '\x90':
                case '\x91': case '\x92': case '\x93': case '\x94': case '\x95': case '\x96': case '\x97': case '\x98': case '\x99':
                case '\x9A': case '\x9B': case '\x9C': case '\x9D': case '\x9E': case '\x9F': case '\xA0': case '\xA1': case '\xA2':
                case '\xA3': case '\xA4': case '\xA5': case '\xA6': case '\xA7': case '\xA8': case '\xA9': case '\xAA': case '\xAB':
                case '\xAC': case '\xAD': case '\xAE': case '\xAF': case '\xB0': case '\xB1': case '\xB2': case '\xB3': case '\xB4':
                case '\xB5': case '\xB6': case '\xB7': case '\xB8': case '\xB9': case '\xBA': case '\xBB': case '\xBC': case '\xBD':
                case '\xBE': case '\xBF': case '\xC0': case '\xC1': case '\xC2': case '\xC3': case '\xC4': case '\xC5': case '\xC6':
                case '\xC7': case '\xC8': case '\xC9': case '\xCA': case '\xCB': case '\xCC': case '\xCD': case '\xCE': case '\xCF':
                case '\xD0': case '\xD1': case '\xD2': case '\xD3': case '\xD4': case '\xD5': case '\xD6': case '\xD7': case '\xD8':
                case '\xD9': case '\xDA': case '\xDB': case '\xDC': case '\xDD': case '\xDE': case '\xDF': case '\xE0': case '\xE1':
                case '\xE2': case '\xE3': case '\xE4': case '\xE5': case '\xE6': case '\xE7': case '\xE8': case '\xE9': case '\xEA':
                case '\xEB': case '\xEC': case '\xED': case '\xEE': case '\xEF': case '\xF0': case '\xF1': case '\xF2': case '\xF3':
                case '\xF4': case '\xF5': case '\xF6': case '\xF7': case '\xF8': case '\xF9': case '\xFA': case '\xFB': case '\xFC':
                case '\xFD': case '\xFE': //BCAF
                    return true;
            }
            return false;
        }

        /*
                public DecoratedName(ulong num) : this(null, num)
                {
                }
        */

        internal DecoratedName(Element parent, ulong num)
            : this(parent)
        {
            this.DoString(num.ToString(CultureInfo.InvariantCulture));
        }

        /*
                public DecoratedName(long num) : this(null, num)
                {
                }
        */

        public DecoratedName(Element parent, long num)
            : this(parent)
        {
            this.DoString(num.ToString(CultureInfo.InvariantCulture));
        }

        private Element Parent { get; set; }

        public IEnumerable<DecoratedNameNode> Nodes
        {
            get
            {
                var cur = this.node;
                while (cur != null)
                {
                    var nrn = cur as NameReferenceNode;
                    if (nrn != null)
                        foreach (var n in nrn.Reference.Nodes)
                            yield return n;
                    else
                        yield return cur;
                    cur = cur.NextNode;
                }
            }
        }

        internal NodeStatus Status
        {
            get
            {
                return this.status & NodeStatus.BasicStatus; // &NodeStatus.PublicStatus;
            }
        }

        internal bool IsValid
        {
            get { return ((this.Status == NodeStatus.None) || (this.Status == NodeStatus.Truncated)); }
        }

        internal bool IsMissing
        {
            get { return this.status == NodeStatus.Truncated; }
        }

        internal bool IsEmpty
        {
            get { return (this.node == null) || (!this.IsValid); }
        }

        internal char LastCharacter
        {
            get
            {
                if (this.IsEmpty)
                    return '\0';
                var n = this.node;
                while (n.NextNode != null)
                    n = n.NextNode;
                return n.LastCharacter;
            }
        }

        internal bool IsArray
        {
            get { return (this.status & NodeStatus.Array) != 0; }
            set
            {
                this.status = value
                                  ? this.status | NodeStatus.Array
                                  : this.status & ~NodeStatus.Array;
            }
        }

        internal bool IsComArray
        {
            get { return (this.status & NodeStatus.ComArray) != 0; }
            set
            {
                this.status = value
                                  ? this.status | NodeStatus.ComArray
                                  : this.status & ~NodeStatus.ComArray;
            }
        }

        internal bool IsNoTypeEncoding
        {
            get { return (this.status & NodeStatus.NoTypeEncoding) != 0; }
            set
            {
                this.status = value
                                  ? this.status | NodeStatus.NoTypeEncoding
                                  : this.status & ~NodeStatus.NoTypeEncoding;
            }
        }

        internal bool IsPinnedPointer
        {
            get { return (this.status & NodeStatus.PinPointer) != 0; }
            set
            {
                this.status = value
                                  ? this.status | NodeStatus.PinPointer
                                  : this.status & ~NodeStatus.PinPointer;
            }
        }

        internal bool IsPointerReference
        {
            get { return (this.status & NodeStatus.PointerReference) != 0; }
            set
            {
                this.status = value
                                  ? this.status | NodeStatus.PointerReference
                                  : this.status & ~NodeStatus.PointerReference;
            }
        }

        public bool IsUdc
        {
            get { return !this.IsEmpty && (this.status & NodeStatus.Udc) != 0; }
            set
            {
                if (!this.IsEmpty)
                    this.status = value
                                      ? this.status | NodeStatus.Udc
                                      : this.status & ~NodeStatus.Udc;
            }
        }

        internal bool IsUdtThunk
        {
            get { return !this.IsEmpty && ((this.status & NodeStatus.UdtThunk) != 0); }
        }

        internal bool IsVirtualCallThunk
        {
            get { return (this.status & NodeStatus.VirtualCallThunk) != 0; }
            set
            {
                this.status = value
                                  ? this.status | NodeStatus.VirtualCallThunk
                                  : this.status & ~NodeStatus.VirtualCallThunk;
            }
        }

        internal int Length
        {
            get
            {
                if (this.IsEmpty)
                    return 0;
                var len = 0;
                for (var n = this.node; n != null; n = n.NextNode)
                    len += n.Length;
                return len;
            }
        }

        public IEnumerator<DecoratedNameNode> GetEnumerator()
        {
            return (IEnumerator<DecoratedNameNode>)this.Nodes;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator<DecoratedNameNode>)this.Nodes;
        }

        internal static DecoratedName CreateReference(DecoratedName pd)
        {
            return CreateReference(null, pd);
        }

        internal static DecoratedName CreateReference(Element parent, DecoratedName pd)
        {
            var result = new DecoratedName(parent);
            if (pd == null)
                return result;
            result.node = new NameReferenceNode(parent, pd);
            result.status = (result.status & 0 /*PublicStatus*/); //| NodeStatus.Invalid;
            return result;
        }

        /*
                public DecoratedName ClearStatus()
                {
                    this.status &= ~NodeStatus.BasicStatus;
                    return this;
                }
        */

        private DecoratedName Add(DecoratedName rd)
        {
            var local = new DecoratedName(this.Parent, this);
            if (local.IsEmpty)
                local.Assign(rd);
            else if (rd.IsEmpty)
                local.Append(rd.Status);
            else
                local.Append(rd);
            return new DecoratedName(this.Parent, local);
        }

        public static DecoratedName operator +(DecoratedName left, DecoratedName right)
        {
            var local = new DecoratedName(left.Parent, left);
            if (local.IsEmpty)
                local.Assign(right);
            else if (right.IsEmpty)
                local.Append(right.status);
            else
                local.Append(right);
            return local;
        }

        public static DecoratedName operator +(DecoratedName left, string right)
        {
            return left.Append(right);
        }

        public static DecoratedName operator +(DecoratedName left, NodeStatus right)
        {
            return left.Append(right);
        }

        public static DecoratedName operator +(DecoratedName left, char right)
        {
            return left.Append(right);
        }

        private static DecoratedName Add(NodeStatus st, DecoratedName rd)
        {
            return new DecoratedName(rd.Parent, st).Add(rd);
        }

        private static DecoratedName Add(string s, DecoratedName rd)
        {
            return new DecoratedName(rd.Parent, s).Add(rd);
        }

        private static DecoratedName Add(char c, DecoratedName rd)
        {
            return new DecoratedName(rd.Parent, c).Add(rd);
        }

        public static DecoratedName operator +(NodeStatus left, DecoratedName right)
        {
            return Add(left, right);
        }

        public static DecoratedName operator +(string left, DecoratedName right)
        {
            return Add(left, right);
        }

        public static DecoratedName operator +(char left, DecoratedName right)
        {
            return Add(left, right);
        }

        public static implicit operator DecoratedName(NodeStatus status)
        {
            return new DecoratedName(status);
        }

        private void Assign(NodeStatus st)
        {
            switch (st)
            {
                case NodeStatus.Error:
                case NodeStatus.Invalid:
                    this.node = null;
                    if (this.Status != NodeStatus.Error)
                        this.status = st & NodeStatus.BasicStatus |
                                      this.status & NodeStatus.ExtendedStatus;
                    break;
                case NodeStatus.None:
                case NodeStatus.Truncated:
                    this.status &= this.status & NodeStatus.TypeStatus;
                    this.node = new DecoratedNameStatusNode(this.Parent, st);
                    break;
            }
        }

        internal void Assign(DecoratedName rd)
        {
            if ((this.Status != NodeStatus.None) && (this.Status != NodeStatus.Truncated))
                return;
            this.status = rd.status & NodeStatus.FullTypeStatus
                          | this.status & ~NodeStatus.FullTypeStatus;
            this.node = rd.node;
        }

        internal void Assign(char c)
        {
            this.DoString(c.ToString(CultureInfo.InvariantCulture));
        }

        internal DecoratedName Assign(string str)
        {
            this.DoString(str);
            return this;
        }

        public DecoratedName Append(char c)
        {
            if (this.node != null)
                this.node.Append(new CharacterNode(this.Parent, c));
            else
                this.node = new CharacterNode(this.Parent, c);
            return this;
        }

        internal DecoratedName Append(string str)
        {
            if (this.node != null)
                this.node.Append(new StringNode(this.Parent, str));
            else
                this.node = new StringNode(this.Parent, str);
            return this;
        }

        internal DecoratedName Append(NodeStatus st)
        {
            if (this.IsEmpty) // || (st == NodeStatus.Missing) || (st == NodeStatus.Error))
                Assign(st);
            else
            {
                var statusNode = new DecoratedNameStatusNode(this.Parent, st);
                this.node = this.node.Clone();
                if (this.node != null)
                    this.node.Append(statusNode);
                else
                    this.status = this.status & ~NodeStatus.BasicStatus | NodeStatus.Error;
            }
            return this;
        }

        internal DecoratedName Append(DecoratedName rd)
        {
            if (rd.IsEmpty)
                this.Append(rd.Status);
            else if (this.IsEmpty)
                this.Assign(rd);
            else
            {
                this.node = this.node.Clone();
                this.node.Append(rd.node);
            }
            return this;
        }

        internal void Prepend(char c)
        {
            this.Assign(c + this);
        }

        internal void Prepend(string str)
        {
            this.Assign(str + this);
        }

        internal void Prepend(NodeStatus st)
        {
            Assign(st + this);
        }

        internal void Prepend(DecoratedName rd)
        {
            Assign(rd + this);
        }

        internal void Skip(DecoratedName rd)
        {
            if (this.status != NodeStatus.Error && !rd.IsValid)
                this.status = this.status & ~NodeStatus.BasicStatus |
                              rd.status & NodeStatus.BasicStatus;
        }

        private void DoString(string str)
        {
            if (this.status == NodeStatus.Invalid || this.status == NodeStatus.Error)
                return;
            if (string.IsNullOrEmpty(str))
                this.Assign(NodeStatus.Error);
            else if (str.Length == 1)
                this.node = new CharacterNode(this.Parent, str[0]);
            else
                this.node = new StringNode(this.Parent, str);
        }

        public override string ToString()
        {
            if (this.IsEmpty) 
                return string.Empty;
            var buffer = new StringBuilder(this.Length);
            for (var n = this.node; n != null; n = n.NextNode)
                buffer.Append(n);
            return buffer.ToString();
        }
    }
}