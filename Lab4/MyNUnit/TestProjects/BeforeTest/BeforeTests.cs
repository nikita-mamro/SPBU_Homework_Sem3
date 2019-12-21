using MyNUnit.Attributes;
using System.Threading;

namespace BeforeTest
{
    /// <summary>
    /// класс для тестов аттрибута Before
    /// </summary>
    public class BeforeTests
    {
        public static int TestValue = 0;

        [Before]
        public void AddBefore()
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
