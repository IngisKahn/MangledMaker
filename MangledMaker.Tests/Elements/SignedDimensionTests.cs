namespace MangledMaker.Tests.Elements
{
    using Core.Elements;
    using Xunit;
    
    public class SignedDimensionTests
    {
        [Fact]
        public void SimpleValue()
        {
            SignedDimension ad = new(new Main(new()), false) { Dimension = { Value = 12345 } };
            Assert.Equal("12345", ad.Name.ToString());
        }
        [Fact]
        public void NegativeValue()
        {
            SignedDimension ad = new(new Main(new()), true) { Dimension = { Value = 12345 } };
            Assert.Equal("-12345", ad.Name.ToString());
        }
        [Fact]
        public void SimpleCode()
        {
            SignedDimension ad = new(new Main(new()), false) { Dimension = { Value = 1 } };
            Assert.Equal("0", ad.Code.ToString());
        }
        [Fact]
        public void NegativeCode()
        {
            SignedDimension ad = new(new Main(new()), true) { Dimension = { Value = 2 } };
            Assert.Equal("?1", ad.Code.ToString());
        }
        [Fact]
        public void ToggleProperties()
        {
            SignedDimension ad = new(new Main(new()), false);
            Assert.True(ad.Negative.HasValue);
            Assert.False(ad.Negative.Value);

            Assert.False(ad.FixedSign);

            ad.Negative = true;
            Assert.True(ad.Negative.HasValue);
            Assert.True(ad.Negative.Value);
            ad.FixedSign = true;
            Assert.Null(ad.Negative);
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
                    SignedDimension parsed = new(new Main(new()), ref copy);
                    Assert.Equal(" ?? ", parsed.Name.ToString());
                }
            }
        }

        [Fact]
        public void SimpleParse()
        {
            unsafe
            {
                const string code = "0";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    SignedDimension parsed = new(new Main(new()), ref copy);
                    Assert.Equal("1", parsed.Name.ToString());
                }
            }
        }

        [Fact]
        public void NegativeParse()
        {
            unsafe
            {
                const string code = "?0";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    SignedDimension parsed = new(new Main(new()), ref copy);
                    Assert.Equal("-1", parsed.Name.ToString());
                }
            }
        }
    }
}