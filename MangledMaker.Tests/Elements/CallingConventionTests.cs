namespace MangledMaker.Tests.Elements
{
    using System;
    using Core;
    using Core.Elements;
    using Xunit;

    public class CallingConventionTests
    {
        [Fact]
        public void NoKeywordsHasEmptyName()
        {
            UnDecorator u = new(DisableOptions.NoMicrosoftKeywords);
            CallingConvention name = new(new Main(u)) { Convention = CallingConvention.ConventionType.C };
            Assert.Equal(string.Empty, name.Name.ToString());
        }
        [Fact]
        public void NoUnderscoreCName()
        {
            UnDecorator u = new(DisableOptions.NoLeadingUnderscores);
            CallingConvention name = new(new Main(u)) { Convention = CallingConvention.ConventionType.C };
            Assert.Equal("cdecl", name.Name.ToString());
        }
        [Fact]
        public void CName()
        {
            CallingConvention name = new(new Main(new())) { Convention = CallingConvention.ConventionType.C };
            Assert.Equal("__cdecl", name.Name.ToString());
        }
        [Fact]
        public void PascalName()
        {
            CallingConvention name = new(new Main(new())) { Convention = CallingConvention.ConventionType.Pascal };
            Assert.Equal("__pascal", name.Name.ToString());
        }
        [Fact]
        public void ThisName()
        {
            CallingConvention name = new(new Main(new())) { Convention = CallingConvention.ConventionType.This };
            Assert.Equal("__thiscall", name.Name.ToString());
        }
        [Fact]
        public void StandardName()
        {
            CallingConvention name = new(new Main(new())) { Convention = CallingConvention.ConventionType.Standard };
            Assert.Equal("__stdcall", name.Name.ToString());
        }
        [Fact]
        public void FastName()
        {
            CallingConvention name = new(new Main(new())) { Convention = CallingConvention.ConventionType.Fast };
            Assert.Equal("__fastcall", name.Name.ToString());
        }
        [Fact]
        public void ClrName()
        {
            CallingConvention name = new(new Main(new())) { Convention = CallingConvention.ConventionType.CommonLanguageRuntime };
            Assert.Equal("__clrcall", name.Name.ToString());
        }
        [Fact]
        public void InvalidName()
        {
            CallingConvention name = new(new Main(new())) { Convention = (CallingConvention.ConventionType)(-1) };
            Assert.Throws<InvalidOperationException>(() => name.Name.ToString());
        }

        [Fact]
        public void CCode()
        {
            CallingConvention code = new(new Main(new())) { Convention = CallingConvention.ConventionType.C };
            Assert.Equal("A", code.Code.ToString());
        }

        [Fact]
        public void PascalCode()
        {
            CallingConvention code = new(new Main(new())) { Convention = CallingConvention.ConventionType.Pascal };
            Assert.Equal("C", code.Code.ToString());
        }

        [Fact]
        public void ClrCode()
        {
            CallingConvention code = new(new Main(new())) { Convention = CallingConvention.ConventionType.CommonLanguageRuntime };
            Assert.Equal("M", code.Code.ToString());
        }
        [Fact]
        public void InvalidCode()
        {
            CallingConvention name = new(new Main(new())) { Convention = (CallingConvention.ConventionType)(-1) };
            Assert.Throws<InvalidOperationException>(() => name.Code.ToString());
        }

        [Fact]
        public void NullParsesTruncated()
        {
            unsafe
            {
                const string code = "\0";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    CallingConvention parsed = new(new Main(new()), ref copy);
                    Assert.Equal(" ?? ", parsed.Name.ToString());
                }
            }
        }

        [Fact]
        public void InvalidParsesEmpty()
        {
            unsafe
            {
                const string code = "N";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    CallingConvention parsed = new(new Main(new()), ref copy);
                    Assert.Equal(string.Empty, parsed.Name.ToString());
                }
            }
        }

        [Fact]
        public void CParses()
        {
            unsafe
            {
                const string code = "A";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    CallingConvention parsed = new(new Main(new()), ref copy);
                    Assert.Equal(CallingConvention.ConventionType.C, parsed.Convention);
                }
            }
        }

        [Fact]
        public void PascalParses()
        {
            unsafe
            {
                const string code = "C";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    CallingConvention parsed = new(new Main(new()), ref copy);
                    Assert.Equal(CallingConvention.ConventionType.Pascal, parsed.Convention);
                }
            }
        }

        [Fact]
        public void ThisParses()
        {
            unsafe
            {
                const string code = "E";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    CallingConvention parsed = new(new Main(new()), ref copy);
                    Assert.Equal(CallingConvention.ConventionType.This, parsed.Convention);
                }
            }
        }

        [Fact]
        public void StandardParses()
        {
            unsafe
            {
                const string code = "G";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    CallingConvention parsed = new(new Main(new()), ref copy);
                    Assert.Equal(CallingConvention.ConventionType.Standard, parsed.Convention);
                }
            }
        }

        [Fact]
        public void FastParses()
        {
            unsafe
            {
                const string code = "I";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    CallingConvention parsed = new(new Main(new()), ref copy);
                    Assert.Equal(CallingConvention.ConventionType.Fast, parsed.Convention);
                }
            }
        }

        [Fact]
        public void ClrParses()
        {
            unsafe
            {
                const string code = "M";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    CallingConvention parsed = new(new Main(new()), ref copy);
                    Assert.Equal(CallingConvention.ConventionType.CommonLanguageRuntime, parsed.Convention);
                }
            }
        }
    }
}