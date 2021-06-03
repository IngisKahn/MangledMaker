namespace MangledMaker.Core
{
    using System.Collections.Generic;

    public sealed class UnDecorator : Replicator
    {
        // private readonly string name;
        private readonly IList<string> parameterList;
        private Replicator argList;

        /*
        private int currentCharacter;
*/

        private DisableOptions disableFlags;

        /*
        private string outputString;
*/
        private Replicator templateArgList;
        private Replicator zNameList = new Replicator();

        public UnDecorator()
            : this(null)
        {
        }

        public UnDecorator(DisableOptions disable)
            : this(null, disable)
        {
        }

        public UnDecorator(IList<string> pGetParameter, DisableOptions disable = DisableOptions.None)
        {
            //this.name = decoratedName + '\0';
            this.ArgList = this;
            this.parameterList = pGetParameter ?? new List<string>();
            this.disableFlags = disable;
        }

        internal Replicator ZNameList
        {
            get { return this.zNameList; }
            set { this.zNameList = value; }
        }

        internal Replicator ArgList
        {
            get { return this.argList; }
            set { this.argList = value; }
        }

        internal Replicator TemplateArgList
        {
            get { return this.templateArgList; }
            set { this.templateArgList = value; }
        }

        internal bool ExplicitTemplateParams { get; set; }

        public bool CanGetTemplateArgumentList { get; set; }

        public DisableOptions DisableFlags
        {
            get { return this.disableFlags; }
            set { this.disableFlags = value; }
        }

        public override void Reset()
        {
            base.Reset();
            this.ExplicitTemplateParams = false;
            this.CanGetTemplateArgumentList = false;
            this.zNameList = new Replicator();
            this.argList = this;
            this.templateArgList = new Replicator();
        }

        public string GetParameter(int index)
        {
            return this.parameterList.Count > index ? this.parameterList[index] : null;
        }

        #region DoFlags

        public bool DoAccessSpecifiers
        {
            get { return (this.disableFlags & DisableOptions.NoAccessSpecifiers) == 0; }
        }

        public bool DoAllocationModel
        {
            get { return (this.disableFlags & DisableOptions.NoAllocationModel) == 0; }
        }

        public bool DoAllocationLanguage
        {
            get { return (this.disableFlags & DisableOptions.NoAllocationLanguage) == 0; }
        }

        public bool DoEcsu
        {
            get { return (this.disableFlags & DisableOptions.NoComplexType) == 0; }
        }

        public bool DoEllipsis
        {
            get { return (this.disableFlags & DisableOptions.NoEllipsis) == 0; }
        }

        public bool DoFunctionReturns
        {
            get { return (this.disableFlags & DisableOptions.NoFunctionReturns) == 0; }
        }

        public bool DoMemberTypes
        {
            get { return (this.disableFlags & DisableOptions.NoMemberType) == 0; }
        }

        public bool DoMicrosoftKeywords
        {
            get { return (this.disableFlags & DisableOptions.NoMicrosoftKeywords) == 0; }
        }

        public bool DoNameOnly
        {
            get { return (this.disableFlags & DisableOptions.NameOnly) != 0; }
        }

        public bool DoNoIdentCharCheck
        {
            get { return (this.disableFlags & DisableOptions.NoIdentCharCheck) == 0; }
        }

        public bool DoPtr64
        {
            get { return (this.disableFlags & DisableOptions.NoPtr64) == 0; }
        }

        public bool DoThisTypes
        {
            get { return (this.disableFlags & DisableOptions.NoThisType) == 0; }
        }

        public bool DoThrowTypes
        {
            get { return (this.disableFlags & DisableOptions.NoThrowSignatures) == 0; }
        }

        public bool DoTypeOnly
        {
            get { return (this.disableFlags & DisableOptions.NoArguments) != 0; }
        }

        public bool DoUnderscore
        {
            get { return (this.disableFlags & DisableOptions.NoLeadingUnderscores) == 0; }
        }

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
        public bool HaveTemplateParameters
        {
            get { return (this.disableFlags & DisableOptions.NoSpecialSymbols) != 0; }
        }

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