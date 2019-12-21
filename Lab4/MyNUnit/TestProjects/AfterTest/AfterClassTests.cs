using MyNUnit.Attributes;

namespace AfterTest
{
    /// <summary>
    /// Класс для тестов аттрибута AfterClass
    /// </summary>
    public class AfterClassTests
    {
        public static int TestValue = 0;

        [AfterClass]
        public static void AddAfterClass()
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
