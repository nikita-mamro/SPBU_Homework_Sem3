using MyNUnit.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestResult
{
    public class ExceptionTests
    {
        [Test("ignore")]
        public void IgnoreException()
            => throw new Exception();

        [Test]
        public void FailException()
            => throw new Exception();
    }
}
