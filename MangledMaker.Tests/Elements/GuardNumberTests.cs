namespace MangledMaker.Tests.Elements
{
    using Core.Elements;
    using Xunit;
    
    public class GuardNumberTests
    {
        [Fact]
        public void SimpleValue()
        {
            GuardNumber d = new(new Main(new())) { Number = { Value = 12345 } };
            Assert.Equal("12345", d.Name.ToString());
        }
        [Fact]
        public void SimpleCode()
        {
            GuardNumber d = new(new Main(new())) { Number = { Value = 1 } };
            Assert.Equal("0", d.Code.ToString());
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
                    GuardNumber parsed = new(new Main(new()), ref copy);
                    Assert.Equal("1", parsed.Name.ToString());
                }
            }
        }
    }
}