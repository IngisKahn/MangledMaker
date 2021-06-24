namespace MangledMaker.Tests.Elements.Attributes
{
    using Core.Elements.Attributes;
    using Xunit;

    public class SettingAttributeTests
    {
        [Fact]
        public void HasRangeFrom()
        {
            var uut = new SettingAttribute { RangeFrom = 3 };
            Assert.Equal(3, uut.RangeFrom);
        }
        [Fact]
        public void HasRangeTo()
        {
            var uut = new SettingAttribute { RangeTo = 3 };
            Assert.Equal(3, uut.RangeTo);
        }
        [Fact]
        public void HasMaxLength()
        {
            var uut = new SettingAttribute { MaxLength = 3 };
            Assert.Equal(3, uut.MaxLength);
        }
    }
}