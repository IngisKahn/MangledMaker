namespace MangledMaker.Tests.Elements
{
    using Core.Elements;
    using Xunit;
    
    public class StringEncodingTests
    {
        [Fact]
        public void SimpleValue()
        {
            StringEncoding ad = new(new Main(new()), "foo", "bar")
            {
                Length = { Value = 3 },
                Checksum = { Value = 33 }
            };
            Assert.Equal("foo", ad.Name.ToString());
            ad.Value = "biz";
            Assert.Equal("foo", ad.Name.ToString());
        }

        [Fact]
        public void SimpleCode()
        {
            StringEncoding ad = new(new Main(new()), "foo", "bar")
            {
                Length = { Value = 3 },
                Checksum = { Value = 33 }
            };
            Assert.Equal("@_02CB@bar@", ad.Code.ToString());
        }

        [Fact]
        public void NullParsesEmpty()
        {
            unsafe
            {
                const string code = "\0";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    StringEncoding parsed = new(new Main(new()), ref copy, "foo");
                    Assert.Equal(string.Empty, parsed.Name.ToString());
                }
            }
        }

        [Fact]
        public void NoUnderscoreParsesEmpty()
        {
            unsafe
            {
                const string code = "@foo";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    StringEncoding parsed = new(new Main(new()), ref copy, "foo");
                    Assert.Equal(string.Empty, parsed.Name.ToString());
                }
            }
        }

        [Fact]
        public void TruncatedParse()
        {
            unsafe
            {
                const string code = "@_022bar\0";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    StringEncoding parsed = new(new Main(new()), ref copy, "foo");
                    Assert.Equal(" ?? ", parsed.Name.ToString());
                }
            }
        }

        [Fact]
        public void SimpleParse()
        {
            unsafe
            {
                const string code = "@_022bar@";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    StringEncoding parsed = new(new Main(new()), ref copy, "foo");
                    Assert.Equal("bar", parsed.Value);
                }
            }
        } 
    }
}