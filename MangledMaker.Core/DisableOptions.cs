namespace MangledMaker.Core
{
    using System;

    [Flags]
    public enum DisableOptions
    {
        None,
        NoLeadingUnderscores,
        NoMicrosoftKeywords,
        NoFunctionReturns = 0x4,
        NoAllocationModel = 0x8,
        NoAllocationLanguage = 0x10,
        NoMicrosoftThisType = 0x20,
        NoCodeViewThisType = 0x40,
        NoThisType = NoMicrosoftThisType | NoCodeViewThisType,
        NoAccessSpecifiers = 0x80,
        NoThrowSignatures = 0x100,
        NoMemberType = 0x200,
/*
        NoReturnUdtModel = 0x400,
*/
/*
        Decode32Bit = 0x800,
*/
        NameOnly = 0x1000,
        NoArguments = 0x2000,
        NoSpecialSymbols = 0x4000,
        NoComplexType = 0x8000,
        NoIdentCharCheck = 0x10000,
        NoPtr64 = 0x20000,
        NoEllipsis = 0x40000
    }
}