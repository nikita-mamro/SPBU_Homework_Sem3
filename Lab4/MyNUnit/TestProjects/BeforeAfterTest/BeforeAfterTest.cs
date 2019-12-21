using MyNUnit.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeforeAfterTest
{
    /// <summary>
    /// Класс для тестов аттрибутов Before/After
    /// </summary>
    public class BeforeAfterTest
    {
        [Before]
        public void Before() { }

        [After]
        public void After() { }

        [BeforeClass]
        public static void BeforeClass() { }

        [AfterClass]
        public static void AfterClass() { }

        [Test]
        public void Test() { }

        [Test]
        public void AnotherTest() { }
    }
}
