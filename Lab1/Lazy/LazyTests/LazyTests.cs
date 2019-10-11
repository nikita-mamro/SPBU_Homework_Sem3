using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Lazy.Tests
{
    /// <summary>
    /// Тесты однопоточной версии Lazy
    /// </summary>
    [TestClass]
    public class LazyTests
    {
        [TestMethod]
        public void LazyContainsNeededValueTest()
        {
            var value = "testOne";
            Func<string> supplier = () => value;
            var lazy = LazyFactory<string>.CreateLazy(supplier);
            Assert.AreEqual(value, lazy.Get());
        }

        [TestMethod]
        public void GetAlwaysReturnsNeededValueTest()
        {
            var value = "testTwo";
            Func<string> supplier = () => value;
            var lazy = LazyFactory<string>.CreateLazy(supplier);

            var firstGet = lazy.Get();
            var secondGet = lazy.Get();

            Assert.AreEqual(firstGet, secondGet);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSupplierExceptionTest()
        {
            Func<string> supplier = null;
            var lazy = LazyFactory<string>.CreateLazy(supplier);
        }

        [TestMethod]
        public void LazyCalculationsTest()
        {
            var calculationsCounter = 0;

            Func<int> supplier = () =>
            {
                ++calculationsCounter;
                return calculationsCounter;
            };

            var lazy = LazyFactory<int>.CreateLazy(supplier);

            var firstGet = lazy.Get();
            var secondGet = lazy.Get();

            Assert.AreEqual(1, calculationsCounter);
        }
    }
}
