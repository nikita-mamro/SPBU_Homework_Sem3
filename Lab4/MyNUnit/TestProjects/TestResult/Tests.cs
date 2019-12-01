using System;
using MyNUnit.Attributes;

namespace TestResult
{
    public class Tests
    {
        public Tests() { }

        [Test]
        public void Success() { }

        [Test("ignore")]
        public void Ignore() { }

        [Test("ignore")]
        public void IgnoreException()
            => throw new Exception();

        [Test]
        public void FailException()
            => throw new Exception();
    }
}
