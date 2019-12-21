using MyNUnit.Attributes;

namespace AfterTest
{
    /// <summary>
    /// Класс для тестов аттрибута After
    /// </summary>
    public class AfterTests
    {

        [After]
        public void AddAfter() { }

        [Test]
        public void Test() { }

        [Test]
        public void AnotherTest() { }
    }
}
