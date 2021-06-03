//namespace MangledMaker.Core
//{
//    using System;

//    [Flags]
//    internal enum EncodingType
//    {
//        /// <summary>
//        ///   0
//        /// </summary>
//        Empty,
//        /// <summary>
//        ///   0
//        /// </summary>
//        FieldPublic = Empty,
//        MemberPublic = Empty,
//        /// <summary>
//        ///   40
//        /// </summary>
//        MemberPrivate = 0x40,
//        /// <summary>
//        ///   80
//        /// </summary>
//        MemberProtected = 0x80,
//        MemberAccess = 0xC0,
//        /// <summary>
//        ///   100
//        /// </summary>
//        Virtual = 0x100,
//        /// <summary>
//        ///   200
//        /// </summary>
//        Static = 0x200,
//        /// <summary>
//        ///   400
//        /// </summary>
//        MemberThunk = 0x400,
//        VtorThunk = 0x500,
//        VtorThunkEx = 0x600,
//        MemberTypes = 0x700,
//        /// <summary>
//        ///   800
//        /// </summary>
//        Member = 0x800,
//        /// <summary>
//        ///   800
//        /// </summary>
//        FieldPrivate = Member,
//        /// <summary>
//        ///   1000
//        /// </summary>
//        FieldProtected = 0x1000,
//        DestructorHelper = 0x1000,
//        /// <summary>
//        ///   1000
//        /// </summary>
//        GlobalThunk = 0x1000,
//        DataMemberConstructorHelper = 0x1100,
//        DataMemberDestructorHelper = 0x1200,
//        FieldAccess = 0x1800,
//        FunctionTypes = 0x1800,
//        HelperFunctions = 0x1B00,
//        /// <summary>
//        ///   2000
//        /// </summary>
//        Far = 0x2000,
//        /// <summary>
//        ///   2000
//        /// </summary>
//        GlobalUnknown = Far,
//        /// <summary>
//        ///   4000
//        /// </summary>
//        Based = 0x4000,
//        /// <summary>
//        ///   4000
//        /// </summary>
//        Global = Based,
//        FieldTypesGlobal = 0x6000,
//        Guard = 0x6000,
//        VirtualFunctionTable = 0x6800,
//        VirtualBaseTable = 0x7000,
//        Name = 0x7800,
//        VirtualDisplacementMap = 0x7C00,
//        SpecialTypes = 0x7C00,
//        /// <summary>
//        ///   8000
//        /// </summary>
//        Function = 0x8000,
//        TypedThunk = 0x9800,

//        /// <summary>
//        ///   FFFD
//        /// </summary>
//        None = 0xFFFD,
//        /// <summary>
//        ///   FFFE
//        /// </summary>
//        Truncated,
//        /// <summary>
//        ///   FFFF
//        /// </summary>
//        Invalid,
//        /// <summary>
//        ///   10000
//        /// </summary>
//        ExternC
//    }
//}

namespace MangledMaker.Core.Elements
{
    using Attributes;

    public sealed class TypeEncoding : Element
    {
        public enum AccessSpecifier
        {
            Public,
            Protected,
            Private
        }

        public enum EncodingType
        {
            GlobalFunction,
            MemberFuntion,
            VirtualConstructorThunk,
            VirtualConstructorThunkExtended,
            LocalStaticDestructorHelper,
            TypedThunk,
            VirtualDisplacementMap,
            TemplateMemberConstructorHelper,
            TemplateMemberDesructorHelper,
            StaticField,
            Global,
            GlobalFlagged,
            Guard,
            VirtualFunctionTable,
            VirtualBaseTable,
            Name,
            NoEncoding
        }

        public enum FunctionModifier
        {
            None,
            Static,
            Virtual,
            VirtualThunk
        }

        private AccessSpecifier access;

        private bool isBased;
        private FunctionModifier modifier;

        public TypeEncoding(Element parent) : base(parent)
        {
        }

        public unsafe TypeEncoding(Element parent, ref char* pSource)
            : base(parent)
        {
            this.Parse(ref pSource);
        }

        [Setting(MaxLength = 9)]
        public string ExternSymbol { get; set; }

        [Setting]
        public EncodingType Type { get; set; }

        [Setting]
        public AccessSpecifier? Access
        {
            get
            {
                switch (this.Type)
                {
                    case EncodingType.MemberFuntion:
                    case EncodingType.StaticField:
                        return this.access;
                }
                return null;
            }
            set { if (value != null) this.access = (AccessSpecifier)value; }
        }

        [Setting]
        public FunctionModifier? Modifier
        {
            get { return this.Type == EncodingType.MemberFuntion ? this.modifier : (FunctionModifier?)null; }
            set { if (value != null) this.modifier = (FunctionModifier)value; }
        }

        [Setting]
        public bool IsFar { get; set; }

        [Setting]
        public bool? IsBased
        {
            get
            {
                switch (this.Type)
                {
                    case EncodingType.GlobalFunction:
                    case EncodingType.MemberFuntion:
                    case EncodingType.TypedThunk:
                    case EncodingType.VirtualConstructorThunk:
                    case EncodingType.VirtualConstructorThunkExtended:
                        return this.isBased;
                    default:
                        return null;
                }
            }
            set { if (value != null) this.isBased = (bool)value; }
        }

        public bool IsClass
        {
            get
            {
                switch (this.Type)
                {
                    case EncodingType.StaticField:
                    case EncodingType.MemberFuntion:
                    case EncodingType.TypedThunk:
                    case EncodingType.VirtualConstructorThunk:
                    case EncodingType.VirtualConstructorThunkExtended:
                        return true;
                }
                return false;
            }
        }

        private bool isExternC;

        [Setting]
        public bool? IsExternC
        {
            get { return string.IsNullOrEmpty(this.ExternSymbol) ? (bool?)null : this.isExternC; }
            set { if (value != null) this.isExternC = (bool)value; }
        }

        public bool IsFunction
        {
            get
            {
                switch (this.Type)
                {
                    case EncodingType.GlobalFunction:
                    case EncodingType.MemberFuntion:
                    case EncodingType.LocalStaticDestructorHelper:
                    case EncodingType.TemplateMemberConstructorHelper:
                    case EncodingType.TemplateMemberDesructorHelper:
                    case EncodingType.TypedThunk:
                    case EncodingType.VirtualConstructorThunk:
                    case EncodingType.VirtualConstructorThunkExtended:
                        return true;
                }
                return false;
            }
        }

        public bool IsGuard
        {
            get { return this.Type == EncodingType.Guard; }
        }

        public bool IsNone
        {
            get { return this.Type == EncodingType.NoEncoding; }
        }

        public bool IsDestructorHelper
        {
            get { return this.Type == EncodingType.LocalStaticDestructorHelper; }
        }

        public bool IsPrivate
        {
            get { return this.IsClass && this.access == AccessSpecifier.Private; }
        }

        public bool IsProtected
        {
            get { return this.IsClass && this.access == AccessSpecifier.Protected; }
        }

        public bool IsPublic
        {
            get { return this.IsClass && this.access == AccessSpecifier.Public; }
        }

        public bool IsDataMemberDestructorHelper
        {
            get { return this.Type == EncodingType.TemplateMemberDesructorHelper; }
        }

        public bool IsName
        {
            get { return this.Type == EncodingType.Name; }
        }

        public bool IsMemberStatic
        {
            get { return this.IsClass && (!this.IsFunction || this.modifier == FunctionModifier.Static); }
        }

        public bool IsThunk
        {
            get
            {
                switch (this.Type)
                {
                    case EncodingType.MemberFuntion:
                        return this.modifier == FunctionModifier.VirtualThunk;
                    case EncodingType.LocalStaticDestructorHelper:
                    case EncodingType.TemplateMemberConstructorHelper:
                    case EncodingType.TemplateMemberDesructorHelper:
                    case EncodingType.VirtualConstructorThunk:
                    case EncodingType.VirtualConstructorThunkExtended:
                    case EncodingType.TypedThunk:
                        return true;
                }
                return false;
            }
        }

        public bool IsTypedThunk
        {
            get { return this.Type == EncodingType.TypedThunk; }
        }

        public bool IsDataMemberConstructorHelper
        {
            get { return this.Type == EncodingType.TemplateMemberConstructorHelper; }
        }

        public bool IsVirtualConstructor
        {
            get { return this.Type == EncodingType.VirtualConstructorThunk; }
        }

        public bool IsVirtualConstructorExtended
        {
            get { return this.Type == EncodingType.VirtualConstructorThunkExtended; }
        }

        public bool IsVirtualFunctionTable
        {
            get { return this.Type == EncodingType.VirtualFunctionTable; }
        }

        public bool IsVirtualBaseTable
        {
            get { return this.Type == EncodingType.VirtualBaseTable; }
        }

        public bool IsVirtualDisplacementMap
        {
            get { return this.Type == EncodingType.VirtualDisplacementMap; }
        }

        public bool IsMemberVirtual
        {
            get
            {
                switch (this.Type)
                {
                    case EncodingType.MemberFuntion:
                        return this.modifier == FunctionModifier.Virtual ||
                               this.modifier == FunctionModifier.VirtualThunk;
                    case EncodingType.VirtualConstructorThunk:
                    case EncodingType.VirtualConstructorThunkExtended:
                        return true;
                }
                return false;
            }
        }

        protected override DecoratedName GenerateName()
        {
            return new DecoratedName(this);
        }

        private unsafe void Parse(ref char* pSource)
        {
            if (*pSource == '_') // a leading underscore signals a based symbol
            {
                this.isBased = true;
                pSource++;
            }

            // parse a letter signaling a member declaration
            if (*pSource >= 'A' && *pSource <= 'Z')
            {
                var letterValue = *pSource++ - 'A';
                this.IsFar = ((letterValue & 1) != 0); // every other letter signals far
                //B D F H J L N P R T V X Z

                // Y & Z are ignored
                if (letterValue < 0x18) // A B C D E F G H I J K L M N O P Q R S T U V W X
                {
                    this.Type = EncodingType.MemberFuntion;
                    switch (letterValue & 0x18)
                    {
                        case 0x0: // A B C D E F G H
                            this.access = AccessSpecifier.Private;
                            break;
                        case 0x8: // I J K L M N O P
                            this.access = AccessSpecifier.Protected;
                            break;
                        case 0x10: // Q R S T U V W X
                            this.access = AccessSpecifier.Public;
                            break;
                    }
                    switch (letterValue & 6)
                    {
                        case 0: // A B I J Q R
                            this.modifier = FunctionModifier.None;
                            break;
                        case 2: // C D K L S T
                            this.modifier = FunctionModifier.Static;
                            break;
                        case 4: // E F M N U V
                            this.modifier = FunctionModifier.Virtual;
                            break;
                        case 6: // G H O P W X
                            this.modifier = FunctionModifier.VirtualThunk;
                            break;
                    }
                }
                else
                    this.Type = EncodingType.GlobalFunction;
            }
            else
            {
                // a $ special type
                if (*pSource == '$')
                {
                    var dispexFlag = false;
                    switch (*++pSource)
                    {
                        case '\0':
                            this.IsTruncated = true;
                            pSource--;
                            break;
                        case '$': // $$ these all tend to signify an unused outer encoding
                            // ignore P
                            if (pSource[1] == 'P')
                                pSource++;

                            switch (*++pSource)
                            {
                                    // start again
                                case 'F': // $$F
                                case 'H': // $$H
                                case 'L': // $$L
                                case 'M': // $$M
                                    pSource++;
                                    this.Parse(ref pSource);
                                    return;

                                    // these all signal a skipped name for extern c (seems buggy)
                                case 'J': // $$J#xxx
                                case 'N': // $$N#xxx
                                case 'O': // $$O#xxx
                                    pSource++;
                                    if ((*pSource >= '0') && (*pSource <= '9'))
                                    {
                                        var symbolLength = *pSource - ('0' - 1);
                                        this.ExternSymbol = new string(pSource, 1, symbolLength - 1);
                                        pSource += symbolLength;
                                        this.Parse(ref pSource);
                                        this.IsExternC = true;
                                        return;
                                    }
                                    this.IsInvalid = true;
                                    break;
                                case 'Q': // $$Q start again
                                    pSource++;
                                    this.Parse(ref pSource);
                                    return;
                            }
                            break;
                        case 'A': //[thunk]: ?? ?? NAME`local static destructor helper'
                            this.Type = EncodingType.LocalStaticDestructorHelper;
                            break;
                        case 'B': //[thunk]: ?? NAME{ ?? , ?? }' }'
                            this.Type = EncodingType.TypedThunk;
                            break;
                        case 'C': //NAME{for ?? }
                            this.Type = EncodingType.VirtualDisplacementMap;
                            break;
                        case 'D': //[thunk]: NAME`template static data member constructor helper'
                            this.Type = EncodingType.TemplateMemberConstructorHelper;
                            break;
                        case 'E': //[thunk]: NAME`template static data member destructor helper'
                            this.Type = EncodingType.TemplateMemberDesructorHelper;
                            break;

                            // the following are vtordisp thunks
                        case 'R':
                            pSource++;
                            dispexFlag = true;
                            goto case '0';
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                            var numericValue = *pSource - '0';
                            this.Type = dispexFlag
                                            ? EncodingType.VirtualConstructorThunkExtended
                                            : EncodingType.VirtualConstructorThunk;
                            this.IsFar = ((numericValue & 1) != 0);
                            switch (numericValue & 6)
                            {
                                case 0:
                                    this.access = AccessSpecifier.Private;
                                    break;
                                case 2:
                                    this.access = AccessSpecifier.Protected;
                                    break;
                                case 4:
                                    this.access = AccessSpecifier.Public;
                                    break;
                                default:
                                    this.IsInvalid = true;
                                    return;
                            }
                            break;
                        default:
                            this.IsInvalid = true;
                            return;
                    }
                    pSource++;
                }
                else
                {
                    // Globals
                    if (*pSource >= '0' && *pSource <= '8')
                        switch (*pSource++)
                        {
                            case '0':
                                this.Type = EncodingType.StaticField;
                                this.access = AccessSpecifier.Private;
                                break;
                            case '1':
                                this.Type = EncodingType.StaticField;
                                this.access = AccessSpecifier.Protected;
                                break;
                            case '2':
                                this.Type = EncodingType.StaticField;
                                this.access = AccessSpecifier.Public;
                                break;
                            case '3':
                                this.Type = EncodingType.Global;
                                break;
                            case '4':
                                this.Type = EncodingType.GlobalFlagged;
                                break;
                            case '5':
                                this.Type = EncodingType.Guard;
                                break;
                            case '6':
                                this.Type = EncodingType.VirtualFunctionTable;
                                break;
                            case '7':
                                this.Type = EncodingType.VirtualBaseTable;
                                break;
                            case '8':
                                this.Type = EncodingType.Name;
                                break;
                        }
                    else
                    {
                        // 9 is name only, game over
                        if (*pSource == '9')
                        {
                            pSource++;
                            this.Type = EncodingType.NoEncoding;
                        }
                        else if (*pSource != '\0')
                            this.IsInvalid = true;
                        else
                            this.IsTruncated = true;
                    }
                }
            }
        }

        protected override DecoratedName GenerateCode()
        {
            var code = new DecoratedName(this);

            if (this.IsBased != null && (bool)this.IsBased)
                code += '_';
            if (this.IsExternC == true)
                code += "$$O" + this.ExternSymbol.Length + this.ExternSymbol;
            var c = '\0';
            switch (this.Type)
            {
                case EncodingType.GlobalFunction:
                case EncodingType.MemberFuntion:
                    c = 'A';
                    if (this.IsFar)
                        c++;
                    if (this.Type == EncodingType.MemberFuntion)
                    {
                        switch (this.access)
                        {
                            case AccessSpecifier.Protected:
                                c += (char)('I' - 'A');
                                break;
                            case AccessSpecifier.Public:
                                c += (char)('Q' - 'A');
                                break;
                        }
                        switch (this.modifier)
                        {
                            case FunctionModifier.Static:
                                c += (char)2;
                                break;
                            case FunctionModifier.Virtual:
                                c += (char)4;
                                break;
                            case FunctionModifier.VirtualThunk:
                                c += (char)6;
                                break;
                        }
                    }
                    else
                        c += (char)('Y' - 'A');
                    break;
                case EncodingType.VirtualConstructorThunk:
                case EncodingType.VirtualConstructorThunkExtended:
                    code += '$';
                    if (this.Type == EncodingType.VirtualConstructorThunkExtended)
                        code += 'R';
                    c = '0';
                    if (this.IsFar)
                        c++;
                    switch (this.access)
                    {
                        case AccessSpecifier.Protected:
                            c += (char)2;
                            break;
                        case AccessSpecifier.Public:
                            c += (char)4;
                            break;
                    }
                    break;
                case EncodingType.LocalStaticDestructorHelper:
                    code += '$';
                    c = 'A';
                    break;
                case EncodingType.TypedThunk:
                    code += '$';
                    c = 'B';
                    break;
                case EncodingType.VirtualDisplacementMap:
                    code += '$';
                    c = 'C';
                    break;
                case EncodingType.TemplateMemberConstructorHelper:
                    code += '$';
                    c = 'D';
                    break;
                case EncodingType.TemplateMemberDesructorHelper:
                    code += '$';
                    c = 'E';
                    break;
                case EncodingType.StaticField:
                    switch (this.access)
                    {
                        case AccessSpecifier.Private:
                            c = '0';
                            break;
                        case AccessSpecifier.Protected:
                            c = '1';
                            break;
                        case AccessSpecifier.Public:
                            c = '2';
                            break;
                    }
                    break;
                case EncodingType.Global:
                    c = '3';
                    break;
                case EncodingType.GlobalFlagged:
                    c = '4';
                    break;
                case EncodingType.Guard:
                    c = '5';
                    break;
                case EncodingType.VirtualFunctionTable:
                    c = '6';
                    break;
                case EncodingType.VirtualBaseTable:
                    c = '7';
                    break;
                case EncodingType.Name:
                    c = '8';
                    break;
                case EncodingType.NoEncoding:
                    c = '9';
                    break;
            }

            return code + c;
        }
    }
}