using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Lazy.Tests
{
    /// <summary>
    /// Тесты Lazy<T>, корректно работающего в 
    /// многопоточном режиме
    /// </summary>
    [TestClass]
    public class ThreadSafeLazyTests
    {
        [TestMethod]
        public void ThreadSafeLazyTest()
        {
            var value = "multithreadTestOne";
            Func<string> supplier = () => value;
            var lazy = LazyFactory<string>.CreateThreadSafeLazy(supplier);

            var threads = new List<Thread>();

            for (var i = 0; i < 100; ++i)
            {
                threads.Add(new Thread(() =>
                {
                    Assert.AreEqual(value, lazy.Get());
                }));
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThreadSafeNullSupplierExceptionTest()
        {
            Func<string> supplier = null;
            var lazy = LazyFactory<string>.CreateThreadSafeLazy(supplier);
        }

        [TestMethod]
        public void ThreadSafeLazyCalculationsTest()
        {
            var calculationsCounter = 0;

            Func<int> supplier = () =>
            {
                ++calculationsCounter;
                return calculationsCounter;
            };

            var lazy = LazyFactory<int>.CreateThreadSafeLazy(supplier);

            var threads = new List<Thread>();

            for (var i = 0; i < 100; ++i)
            {
                threads.Add(new Thread(() =>
                {
                    var testGet = lazy.Get();
                    Assert.AreEqual(1, lazy.Get());
                }));
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            Assert.AreEqual(1, calculationsCounter);
        }
    }
}