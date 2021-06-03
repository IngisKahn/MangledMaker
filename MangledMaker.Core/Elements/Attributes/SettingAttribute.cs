namespace MangledMaker.Core.Elements.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class SettingAttribute : Attribute
    {
        public int RangeFrom { get; set; }

        public int RangeTo { get; set; }

        public int MaxLength { get; set; }
    }
}