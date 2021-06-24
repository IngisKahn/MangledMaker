namespace MangledMaker.Tests.Elements
{
    using Core.Elements;
    using Xunit;

    public class EnumTypeTests
    {
        [Fact]
        public void CharName()
        {
            EnumType name = new(new Main(new()), EnumType.EnumBaseType.Char);
            Assert.Equal("char ", name.Name.ToString());
        }
        [Fact]
        public void UCharName()
        {
            EnumType name = new(new Main(new()), EnumType.EnumBaseType.Char, false);
            Assert.Equal("unsigned char ", name.Name.ToString());
        }
        [Fact]
        public void ShortName()
        {
            EnumType name = new(new Main(new()), EnumType.EnumBaseType.Short);
            Assert.Equal("short ", name.Name.ToString());
        }
        [Fact]
        public void UShortName()
        {
            EnumType name = new(new Main(new()), EnumType.EnumBaseType.Short, false);
            Assert.Equal("unsigned short ", name.Name.ToString());
        }
        [Fact]
        public void IntName()
        {
            EnumType name = new(new Main(new()), EnumType.EnumBaseType.Int);
            Assert.Equal(string.Empty, name.Name.ToString());
        }
        [Fact]
        public void UIntName()
        {
            EnumType name = new(new Main(new()), EnumType.EnumBaseType.Int, false);
            Assert.Equal("unsigned int ", name.Name.ToString());
        }
        [Fact]
        public void LongName()
        {
            EnumType name = new(new Main(new()), EnumType.EnumBaseType.Long);
            Assert.Equal("long ", name.Name.ToString());
        }
        [Fact]
        public void ULongName()
        {
            EnumType name = new(new Main(new()), EnumType.EnumBaseType.Long, false);
            Assert.Equal("unsigned long ", name.Name.ToString());
        }

        [Fact]
        public void CharCode()
        {
            EnumType code = new(new Main(new()), EnumType.EnumBaseType.Char);
            Assert.Equal("0", code.Code.ToString());
        }

        [Fact]
        public void UnsignedCharCode()
        {
            EnumType code = new(new Main(new()), EnumType.EnumBaseType.Char, false);
            Assert.Equal("1", code.Code.ToString());
        }

        [Fact]
        public void CharParse()
        {
            unsafe
            {
                const string code = "0";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    EnumType uut = new(new Main(new()), ref copy);
                    Assert.Equal(EnumType.EnumBaseType.Char, uut.BaseType);
                    Assert.True(uut.Signed);
                }
            }
        }

        [Fact]
        public void UnsignedCharParse()
        {
            unsafe
            {
                const string code = "1";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    EnumType uut = new(new Main(new()), ref copy);
                    Assert.Equal(EnumType.EnumBaseType.Char, uut.BaseType);
                    Assert.False(uut.Signed);
                }
            }
        }

        [Fact]
        public void ShortParse()
        {
            unsafe
            {
                const string code = "2";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    EnumType uut = new(new Main(new()), ref copy);
                    Assert.Equal(EnumType.EnumBaseType.Short, uut.BaseType);
                    Assert.True(uut.Signed);
                }
            }
        }

        [Fact]
        public void IntParse()
        {
            unsafe
            {
                const string code = "4";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    EnumType uut = new(new Main(new()), ref copy);
                    Assert.Equal(EnumType.EnumBaseType.Int, uut.BaseType);
                    Assert.True(uut.Signed);
                }
            }
        }

        [Fact]
        public void LongParse()
        {
            unsafe
            {
                const string code = "6";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    EnumType uut = new(new Main(new()), ref copy);
                    Assert.Equal(EnumType.EnumBaseType.Long, uut.BaseType);
                    Assert.True(uut.Signed);
                }
            }
        }

        [Fact]
        public void TruncatedParse()
        {
            unsafe
            {
                const string code = "";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    EnumType uut = new(new Main(new()), ref copy);
                    Assert.Equal(" ?? ", uut.Name.ToString());
                }
            }
        }

        [Fact]
        public void InvalidParse()
        {
            unsafe
            {
                const string code = "9";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    EnumType uut = new(new Main(new()), ref copy);
                    Assert.False(uut.Name.IsValid);
                }
            }
        }
    }
}