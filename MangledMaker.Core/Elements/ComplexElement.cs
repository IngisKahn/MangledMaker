namespace MangledMaker.Core.Elements
{
    public abstract class ComplexElement : Element
    {
        protected UnDecorator UnDecorator { get; private set; }

        protected ComplexElement(UnDecorator unDecorator)
            : base(null)
        {
            this.UnDecorator = unDecorator;
        }

        protected ComplexElement(ComplexElement parent)
            : base(parent)
        {
            this.UnDecorator = parent.UnDecorator;
        }

        private static readonly string[] tokenTable = 
        {
#if !VERS_32BIT
	                "__near",		// TOK_near
	                "__near ",		// TOK_nearSp
	                "__near*",		// TOK_nearP
	                "__far",		// TOK_far
	                "__far ",		// TOK_farSp
	                "__far*",		// TOK_farP
	                "__huge",		// TOK_huge
	                "__huge ",		// TOK_hugeSp
	                "__huge*",		// TOK_hugeP
#endif
            "__based(",		// TOK_basedLp
            "__cdecl",		// TOK_cdecl
            "__pascal",		// TOK_pascal
            "__stdcall",	// TOK_stdcall
            "__thiscall",	// TOK_thiscall
            "__fastcall",	// TOK_fastcall
            "__clrcall",
            "__ptr64",		// TOK_ptr64
            "__restrict",	// TOK_restrict
            "__unaligned",
#if !VERS_32BIT
	                "__interrupt",	// TOK_interrupt
	                "__saveregs",	// TOK_saveregs
	                "__self",		// TOK_self
	                "__segment",	// TOK_segment
	                "__segname(\"",	// TOK_segnameLpQ
#endif
            string.Empty
        };


        /// <summary>
        /// A system token
        /// </summary>
        protected enum TokenType
        {
#if !VERS_32BIT
                Near,
                NearStackPointer,
                NearPointer,
                Far,
                FarStackPointer,
                FarPointer,
                Huge,
                HugeStackPointer,
                HugePointer,
#endif
            Based,
            C,
            Pascal,
            Standard,
            This,
            Fast,
            CommonLanguageRuntime,
            Pointer,
            Restrict,
            Unaligned,
#if !VERS_32BIT
                Intterupt,
                SaveRegs,
                Self,
                Segment,
                SegName
#endif
        }

        /// <summary>
        /// Converts a <see cref="TokenType"/> to string.
        /// </summary>
        /// <param name="token">The token desired.</param>
        /// <returns>The specified token prepended with underscores if not disabled.</returns>
        protected string UScore(TokenType token)
        {
            string tokenName;
            switch (token)
            {
                case TokenType.Based:
                case TokenType.C:
                case TokenType.Pascal:
                case TokenType.Standard:
                case TokenType.This:
                case TokenType.Fast:
                case TokenType.CommonLanguageRuntime:
                case TokenType.Pointer:
                case TokenType.Restrict:
                case TokenType.Unaligned:
                    tokenName = tokenTable[token - TokenType.Based];
                    break;
                default:
                    return null;
            }
            if (!this.UnDecorator.DoUnderscore)
                tokenName = tokenName.Substring(2);
            return tokenName;
        }
    }
}