using MyNUnit.Attributes;

namespace TestResult
{
    /// <summary>
    /// Класс для примитивных сценариев тестирования
    /// </summary>
    public class Tests
    {
        [Test]
        public void Success() { }

        [Test("Let's ignore this method")]
        public void Ignore() { }
    }
}
