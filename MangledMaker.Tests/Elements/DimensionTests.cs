namespace MangledMaker.Tests.Elements
{   
    using Xunit;
    using Core.Elements;
    
    public class DimensionTests
    {
        [Fact]
        public void PositiveNumberName()
        {
            Dimension obj = new(new Main(new()), 2);
            Assert.Equal("2", obj.Name.ToString());
        }

        [Fact]
        public void PositiveNumberCode()
        {
            Dimension obj = new(new Main(new()), 2);
            Assert.Equal("1", obj.Code.ToString());
        }
        [Fact]
        public void NegativeNumberName()
        {
            Dimension obj = new(new Main(new()), -12345);
            Assert.Equal("-12345", obj.Name.ToString());
        }

        [Fact]
        public void NegativeNumberCode()
        {
            Dimension obj = new(new Main(new()), -12345);
            Assert.Equal("PPPPPPPPPPPPMPMH@", obj.Code.ToString());
        }

        [Fact]
        public void NonTemplateTypeCode()
        {
            Dimension obj = new(new Main(new())) { IsNonTypeTemplateParameter = true };
            Assert.Equal("Q@", obj.Code.ToString());
            obj.IsNonTypeTemplateParameter = null;
            Assert.Equal("@", obj.Code.ToString());
        }

        [Fact]
        public void NonTemplateTypeName()
        {
            Dimension obj = new(new Main(new())) { IsNonTypeTemplateParameter = true };
            Assert.Equal("`non-type-template-parameter-0", obj.Name.ToString());
            obj.IsNonTypeTemplateParameter = false;
            Assert.Equal("0", obj.Name.ToString()); 
            obj.IsNonTypeTemplateParameter = true;
            obj.Value = 10;
            Assert.Equal("10", obj.Name.ToString());
        }

        [Fact]
        public void NonTemplateTypeAndValueSettings()
        {
            Dimension obj = new(new Main(new()), 2);
            Assert.Equal(2, obj.Value);
            Assert.True(obj.IsNonTypeTemplateParameter.HasValue);
            Assert.False(obj.IsNonTypeTemplateParameter.Value);
            obj.Value = -1;
            Assert.Null(obj.IsNonTypeTemplateParameter);
            obj.Value = 11;
            Assert.Null(obj.IsNonTypeTemplateParameter);
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
                    Dimension parsed = new(new Main(new()), ref copy, false);
                    Assert.Equal(" ?? ", parsed.Name.ToString());
                }
            }
        }

        [Fact]
        public void UnTerminatedParsesTruncated()
        {
            unsafe
            {
                const string code = "AA\0";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    Dimension parsed = new(new Main(new()), ref copy, false);
                    Assert.Equal(" ?? ", parsed.Name.ToString());
                }
            }
        }

        [Fact]
        public void InvalidParsesEmpty()
        {
            unsafe
            {
                const string code = " ";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    Dimension parsed = new(new Main(new()), ref copy, false);
                    Assert.Equal(string.Empty, parsed.Name.ToString());
                }
            }
        }

        [Fact]
        public void Parse0()
        {
            unsafe
            {
                const string code = "@";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    Dimension parsed = new(new Main(new()), ref copy, false);
                    Assert.Equal(0, parsed.Value);
                }
            }
        }

        [Fact]
        public void Parse1()
        {
            unsafe
            {
                const string code = "0";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    Dimension parsed = new(new Main(new()), ref copy, false);
                    Assert.Equal(1, parsed.Value);
                }
            }
        }

        [Fact]
        public void ParseM12345()
        {
            unsafe
            {
                const string code = "PPPPPPPPPPPPMPMH@";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    Dimension parsed = new(new Main(new()), ref copy, true);
                    Assert.Equal(parsed.Value, -12345);
                }
            }
        }

        [Fact]
        public void Parse1Ntt()
        {
            unsafe
            {
                const string code = "Q0";
                fixed (char* pCode = code)
                {
                    var copy = pCode;
                    Dimension parsed = new(new Main(new()), ref copy, false);
                    Assert.Equal(1, parsed.Value);
                    Assert.True(parsed.IsNonTypeTemplateParameter.HasValue);
                    Assert.True(parsed.IsNonTypeTemplateParameter.Value);
                }
            }
        }
    }
}
