namespace MangledMaker.Tests.Elements
{
    using Core.Elements;
    using Xunit;

    public class CallIndexTests
    {
        [Fact]
        public void SimpleValue()
        {
            CallIndex d = new(new Main(new())) { Index = { Value = 12345 } };
            Assert.Equal("12345", d.Name.ToString());
        }
        [Fact]
        public void SimpleCode()
        {
            CallIndex d = new(new Main(new())) { Index = { Value = 1 } };
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
                    CallIndex parsed = new (new Main(new()), ref copy);
                    Assert.Equal("1", parsed.Name.ToString());
                }
            }
        }
    }
}