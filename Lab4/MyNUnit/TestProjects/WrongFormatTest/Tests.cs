using MyNUnit.Attributes;

namespace WrongFormatTest
{
    /// <summary>
    /// Класс для проверки исключений о неверно заданных тестовых методах
    /// </summary>
    public class Tests
    {
        [Test]
        public int IntTest() { return 0; }
    }
}
