using MyNUnit.Attributes;

namespace AfterTest
{
    /// <summary>
    /// Класс для тестов аттрибута After
    /// </summary>
    public class AfterTests
    {
        public static int TestValue = 0;

        [After]
        public void AddAfter()
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
