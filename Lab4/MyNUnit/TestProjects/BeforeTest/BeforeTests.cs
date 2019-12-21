using MyNUnit.Attributes;

namespace BeforeTest
{
    /// <summary>
    /// класс для тестов аттрибута Before
    /// </summary>
    public class BeforeTests
    {
        [Before]
        public void AddBefore() { }

        [Test]
        public void Test() { }

        [Test]
        public void AnotherTest() { }
    }
}
