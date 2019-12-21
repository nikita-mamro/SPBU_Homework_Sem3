using System;
using MyNUnit.Attributes;

namespace TestResult
{
    public class Tests
    {
        [Test]
        public void Success() { }

        [Test("ignore")]
        public void Ignore() { }
    }
}
