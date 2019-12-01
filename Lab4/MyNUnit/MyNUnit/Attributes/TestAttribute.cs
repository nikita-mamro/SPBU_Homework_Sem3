using System;

namespace MyNUnit.Attributes
{
    public class TestAttribute : Attribute
    {
        public Type Exception { get; private set; }
        public string CancellationMessage { get; private set; }
        public bool IsCanceled
            => CancellationMessage == "";

        public TestAttribute(string ignore = "", Type expected = null)
        {
            Exception = expected;
            CancellationMessage = ignore;
        }
    }
}
