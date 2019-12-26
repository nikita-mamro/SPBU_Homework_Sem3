using MyNUnit.Attributes;
using System;

namespace TestResult
{
    /// <summary>
    /// Класс для тестов на исключения
    /// </summary>
    public class ExceptionTests
    {
        [Test("ignore")]
        public void IgnoreException()
            => throw new Exception();

        [Test("", typeof(ArgumentNullException))]
        public void ExpectedException()
            => throw new ArgumentNullException();

        [Test]
        public void FailException()
            => throw new Exception();

        [Test("", typeof(ArgumentNullException))]
        public void UnexpectedException()
            => throw new DivideByZeroException();
    }
}
