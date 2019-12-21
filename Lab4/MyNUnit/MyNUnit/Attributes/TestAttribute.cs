using System;

namespace MyNUnit.Attributes
{
    public class TestAttribute : Attribute
    {
        public Type ExpectedException { get; private set; }
        public string IgnoranceMessage { get; private set; }
        public bool IsIgnored
            => IgnoranceMessage != "";

        public TestAttribute(string ignore = "", Type expected = null)
        {
            ExpectedException = expected;
            IgnoranceMessage = ignore;
        }
    }
}
