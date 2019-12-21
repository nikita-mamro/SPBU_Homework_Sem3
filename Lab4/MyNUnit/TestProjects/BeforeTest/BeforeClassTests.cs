using MyNUnit.Attributes;

namespace BeforeTest
{
    /// <summary>
    /// Класс для тестов аттрибута BeforeClass
    /// </summary>
    public class BeforeClassTests
    {
        public static int TestValue = 0;

        [BeforeClass]
        public static void AddBeforeClass()
        {
            ++TestValue;
        }

        [Test]
        public void Test()
        {
            ++TestValue;
        }

        [Test]
        public void AnotherTest()
        {
            ++TestValue;
        }
    }
}
