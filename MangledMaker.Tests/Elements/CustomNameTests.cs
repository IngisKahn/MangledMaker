namespace MangledMaker.Tests.Elements
{
    using Core;
    using Core.Elements;
    using Xunit;

    public class CustomNameTests
    {
        [Fact]
        public void NoParamName()
        {
            CustomName c = new(new Main(new()), "foo") { Index = { Dimension = { Value = 2 } } };
            Assert.Equal("`foo2'", c.Name.ToString());
        }

        [Fact]
        public void NoValueName()
        {
            UnDecorator u = new() { DisableFlags = DisableOptions.NoSpecialSymbols };

            CustomName c = new(new Main(u), "foo") { Index = { Dimension = { Value = 2 } } };
            Assert.Equal("`foo2'", c.Name.ToString());
        }

        [Fact]
        public void ValueName()
        {
            UnDecorator u = new(new[] { "fee", "fi", "fo", "fum" }, DisableOptions.NoSpecialSymbols);
            CustomName c = new(new Main(u), "foo") { Index = { Dimension = { Value = 2 } } };
            Assert.Equal("fo", c.Name.ToString());
        }
        [Fact]
        public void SimpleCode()
        {
            CustomName c = new(new Main(new()), "foo") { Index = { Dimension = { Value = 2 } } };
            Assert.Equal("1", c.Code.ToString());
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
                    CustomName parsed = new(new Main(new()), ref copy, "foo");
                    Assert.Equal("`foo1'", parsed.Name.ToString());
                }
            }
        }
    }
}