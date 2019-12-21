using MyNUnit.Attributes;

namespace WrongParametersFormatTest
{
    /// <summary>
    /// Класс для проверки исключений о неверно заданных тестовых методах
    /// </summary>
    public class Tests
    {
        [Test]
        public void Method(int a) { }
    }
}
