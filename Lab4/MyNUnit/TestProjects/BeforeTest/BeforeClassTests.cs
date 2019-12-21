using MyNUnit.Attributes;

namespace BeforeTest
{
    /// <summary>
    /// Класс для тестов аттрибута BeforeClass
    /// </summary>
    public class BeforeClassTests
    {
        [BeforeClass]
        public static void AddBeforeClass() { }

        [Test]
        public void Test() { }

        [Test]
        public void AnotherTest() { }
    }
}
