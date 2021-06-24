namespace MangledMaker.Tests.Elements
{
    using Core.Elements;
    using Xunit;
    
    public class DisplacementTests
    {
        [Fact]
        public void SimpleValue()
        {
            Displacement d = new(new Main(new())) { Offset = { Value = 12345 } };
            Assert.Equal("12345", d.Name.ToString());
        }
        [Fact]
        public void SimpleCode()
        {
            Displacement d = new(new Main(new())) { Offset = { Value = 1 } };
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
                    Displacement parsed = new(new Main(new()), ref copy);
                    Assert.Equal("1", parsed.Name.ToString());
                }
            }
        }
    }
}