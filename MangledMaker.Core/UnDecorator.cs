namespace MangledMaker.Core
{
    using System.Collections.Generic;

    public sealed class UnDecorator : Replicator
    {
        // private readonly string name;
        private readonly IList<string> parameterList;

        public UnDecorator()
            : this(null)
        {
        }

        public UnDecorator(DisableOptions disable)
            : this(null, disable)
        {
        }

        public UnDecorator(IList<string>? pGetParameter, DisableOptions disable = DisableOptions.None)
        {
            //this.name = decoratedName + '\0';
            this.ArgList = this;
            this.parameterList = pGetParameter ?? new List<string>();
            this.DisableFlags = disable;
        }

        internal Replicator ZNameList { get; set; } = new();

        internal Replicator ArgList { get; set; }

        internal Replicator TemplateArgList { get; set; } = new();

        internal bool ExplicitTemplateParams { get; set; }

        public bool CanGetTemplateArgumentList { get; set; }

        public DisableOptions DisableFlags { get; set; }

        public override void Reset()
        {
            base.Reset();
            this.ExplicitTemplateParams = false;
            this.CanGetTemplateArgumentList = false;
            this.ZNameList = new();
            this.ArgList = this;
            this.TemplateArgList = new();
        }

        public string? GetParameter(int index) => this.parameterList.Count > index ? this.parameterList[index] : null;

        #region DoFlags

        public bool DoAccessSpecifiers => (this.DisableFlags & DisableOptions.NoAccessSpecifiers) == 0;

        public bool DoAllocationModel => (this.DisableFlags & DisableOptions.NoAllocationModel) == 0;

        public bool DoAllocationLanguage => (this.DisableFlags & DisableOptions.NoAllocationLanguage) == 0;

        public bool DoEcsu => (this.DisableFlags & DisableOptions.NoComplexType) == 0;

        public bool DoEllipsis => (this.DisableFlags & DisableOptions.NoEllipsis) == 0;

        public bool DoFunctionReturns => (this.DisableFlags & DisableOptions.NoFunctionReturns) == 0;

        public bool DoMemberTypes => (this.DisableFlags & DisableOptions.NoMemberType) == 0;

        public bool DoMicrosoftKeywords => (this.DisableFlags & DisableOptions.NoMicrosoftKeywords) == 0;

        public bool DoNameOnly => (this.DisableFlags & DisableOptions.NameOnly) != 0;

        public bool DoNoIdentCharCheck => (this.DisableFlags & DisableOptions.NoIdentCharCheck) == 0;

        public bool DoPtr64 => (this.DisableFlags & DisableOptions.NoPtr64) == 0;

        public bool DoThisTypes => (this.DisableFlags & DisableOptions.NoThisType) == 0;

        public bool DoThrowTypes => (this.DisableFlags & DisableOptions.NoThrowSignatures) == 0;

        public bool DoTypeOnly => (this.DisableFlags & DisableOptions.NoArguments) != 0;

        public bool DoUnderscore => (this.DisableFlags & DisableOptions.NoLeadingUnderscores) == 0;

        /*
        public bool DoReturnUdtModel
        {
            get { return (this.disableFlags & DisableOptions.NoReturnUdtModel) == 0; }
        }
*/

/*
        public bool Do32BitNear
        {
            get { return (this.disableFlags & DisableOptions.Decode32Bit) == 0; }
        }
*/

        /// <summary>
        ///   Returns true if the NoSpecialSymbols flag is set.
        /// </summary>
        public bool HaveTemplateParameters => (this.DisableFlags & DisableOptions.NoSpecialSymbols) != 0;

        #endregion

        //public override string ToString()
        //{
        //    return string.Format("{0} -> {1}", this.name, this.outputString);
        //}

        /*
        private string RemoveDuplicateSpaces(string input)
        {
            var output = new StringBuilder(input);
            bool wasSpace = false;
            foreach (char c in this.outputString)
                if (!wasSpace)
                {
                    wasSpace = c == ' ';
                    output.Append(c);
                }
                else if (c != ' ')
                {
                    wasSpace = false;
                    output.Append(c);
                }
            return output.ToString();
        }
*/
    }
}