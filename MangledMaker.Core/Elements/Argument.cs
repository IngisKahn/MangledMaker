namespace MangledMaker.Core.Elements
{
    using System;
    using Attributes;

    public sealed class Argument : ComplexElement
    {
        public enum ArgumentTypes
        {
            Saved1,
            Saved2,
            Saved3,
            Saved4,
            Saved5,
            Saved6,
            Saved7,
            Saved8,
            Saved9,
            Saved10,
            Type
        }

        public Argument(ComplexElement parent)
            : base(parent)
        {
        }

        public unsafe Argument(ComplexElement parent, ref char* pSource)
            : base(parent) =>
            this.Parse(ref pSource);

        [Setting]
        public ArgumentTypes ArgumentType { get; set; }

        private PrimaryDataType? type;

        [Child] public PrimaryDataType Type
        {
            get => this.type ??= new(this, new());
            private set => this.type = value;
        }

        protected override DecoratedName GenerateName()
        {
            DecoratedName result = new(this);
            switch (this.ArgumentType)
            {
                case ArgumentTypes.Saved1:
                case ArgumentTypes.Saved2:
                case ArgumentTypes.Saved3:
                case ArgumentTypes.Saved4:
                case ArgumentTypes.Saved5:
                case ArgumentTypes.Saved6:
                case ArgumentTypes.Saved7:
                case ArgumentTypes.Saved8:
                case ArgumentTypes.Saved9:
                case ArgumentTypes.Saved10:
                    return this.UnDecorator.ArgList[(int) this.ArgumentType];
                case ArgumentTypes.Type:
                    result.Assign(this.Type.Name);
                    break;
            }

            return result;
        }

        private unsafe void Parse(ref char* pSource)
        {
            var numeric = pSource[0] - 48;
            if (numeric is >= 0 and <= 9)
            {
                pSource++;
                this.ArgumentType = (ArgumentTypes) numeric;
            }
            else
            {
                this.Type = new(this, ref pSource, new());
                this.ArgumentType = ArgumentTypes.Type;
                var result = this.Type.Name;
                if (result.Length > 1 && !this.UnDecorator.ArgList.IsFull)
                    this.UnDecorator.ArgList.Append(this.Type);
            }
        }

        protected override DecoratedName GenerateCode()
        {
            DecoratedName result = new(this);
            switch (this.ArgumentType)
            {
                case ArgumentTypes.Saved1:
                case ArgumentTypes.Saved2:
                case ArgumentTypes.Saved3:
                case ArgumentTypes.Saved4:
                case ArgumentTypes.Saved5:
                case ArgumentTypes.Saved6:
                case ArgumentTypes.Saved7:
                case ArgumentTypes.Saved8:
                case ArgumentTypes.Saved9:
                case ArgumentTypes.Saved10:
                    result.Assign((char) ((int) this.ArgumentType + '0'));
                    break;
                case ArgumentTypes.Type:
                    result.Assign(this.Type.Code ?? throw new InvalidOperationException());
                    break;
            }
            return result;
        }
    }
}