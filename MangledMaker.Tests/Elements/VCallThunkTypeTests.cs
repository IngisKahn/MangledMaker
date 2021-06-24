namespace MangledMaker.Tests.Elements
{
    using Core.Elements;
    using Xunit;
    
    public class VCallThunkTypeTests
    {
        [Fact]
        //[TestCategory("VCallThunkType")]
        public void DefaultIsFlat()
        {
            VCallThunkType obj = new(new Main(new()));
            Assert.Equal("{flat}", obj.Name.ToString());
        }

        [Fact]
        public void FlatGeneratesA()
        {
            VCallThunkType generated = new(new Main(new()));
            Assert.Equal("A", generated.Code.ToString());
        }

        [Fact]
        public void AParsesFlat()
        {
            unsafe
            {
                const string code = "A";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    VCallThunkType parsed = new(new Main(new()), ref copy);
                    Assert.Equal("{flat}", parsed.Name.ToString());
                }
            }
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
                    VCallThunkType parsed = new(new Main(new()), ref copy);
                    Assert.Equal(" ?? ", parsed.Name.ToString());
                }
            }
        }

        [Fact]
        public void InvalidParsesEmpty()
        {
            unsafe
            {
                const string code = "B";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    VCallThunkType parsed = new(new Main(new()), ref copy);
                    Assert.Equal(string.Empty, parsed.Name.ToString());
                }
            }
        }
    }
}