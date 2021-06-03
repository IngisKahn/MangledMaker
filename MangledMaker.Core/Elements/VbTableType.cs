namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class VbTableType : ComplexElement
    {
        public VbTableType(ComplexElement parent, DecoratedName superType)
            : base(parent)
        {
            this.SuperType = superType;
            this.TableType = new VfTableType(this, superType);
        }

        public unsafe VbTableType(ComplexElement parent, ref char* pSource,
                                  DecoratedName superType)
            : base(parent)
        {
            this.SuperType = superType;
            this.Parse(ref pSource);
        }

        [Input]
        public DecoratedName SuperType { get; set; }

        [Child]
        public VfTableType TableType { get; private set; }


        protected override DecoratedName GenerateName()
        {
            this.TableType.SuperType = this.SuperType;
            return this.TableType.Name;
        }

        private unsafe void Parse(ref char* pSource)
        {
            this.TableType = new VfTableType(this, ref pSource, this.SuperType);
        }

        protected override DecoratedName GenerateCode()
        {
            this.TableType.SuperType = this.SuperType;
            return this.TableType.Code;
        }
    }
}