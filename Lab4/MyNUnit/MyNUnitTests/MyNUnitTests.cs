using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MyNUnit.Tests
{
    /// <summary>
    /// Тесты для MyNUnit
    /// </summary>
    [TestClass]
    public class MyNUnitTests
    {
        private List<TestInfo> regularTestsResults;
        private List<string> expectedRegularResultsMethods;

        [TestInitialize]
        public void Initialize()
        {
            regularTestsResults = new List<TestInfo>();
            expectedRegularResultsMethods = new List<string>();
            expectedRegularResultsMethods.Add("Success");
            expectedRegularResultsMethods.Add("Ignore");
            expectedRegularResultsMethods.Add("IgnoreException");
            expectedRegularResultsMethods.Add("ExpectedException");
            expectedRegularResultsMethods.Add("FailException");
            expectedRegularResultsMethods.Add("UnexpectedException");

            var resultsTestPath = "..\\..\\..\\TestProjects\\TestResult\\Assembly";

            var regularTestsReport = MyNUnit.RunTestsAndGetReport(resultsTestPath);

            foreach (var list in regularTestsReport.Values)
            {
                foreach (var info in list)
                {
                    regularTestsResults.Add(info);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void MethodsFormatTest()
        {
            MyNUnit.RunTests("..\\..\\..\\TestProjects\\WrongFormatTest\\Assembly");
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void MethodsParametersFormatTest()
        {
            MyNUnit.RunTests("..\\..\\..\\TestProjects\\WrongParametersFormatTest\\Assembly");
        }

        [TestMethod]
        public void CorrectMethodsAreTestedTest()
        {
            var names = new List<string>();

            foreach (var res in regularTestsResults)
            {
                names.Add(res.MethodName);
            }

            Assert.AreEqual(names.Intersect(expectedRegularResultsMethods).Count(), expectedRegularResultsMethods.Count);
        }

        [TestMethod]
        public void RegularTestPassedTest()
        {
            var successInfo = regularTestsResults.Find(i => i.MethodName == "Success");

            Assert.IsTrue(successInfo.IsPassed);
        }

        [TestMethod]
        public void IgnoreTest()
        {
            var ignoreInfo = regularTestsResults.Find(i => i.MethodName == "Ignore");
            var exceptionIgnoreInfo = regularTestsResults.Find(i => i.MethodName == "IgnoreException");

            Assert.IsTrue(ignoreInfo.IsIgnored);
            Assert.AreEqual("Let's ignore this method", ignoreInfo.IgnoranceMessage);
            Assert.IsTrue(exceptionIgnoreInfo.IsIgnored);
        }

        [TestMethod]
        public void ExpectedExceptionTest()
        {
            var info = regularTestsResults.Find(i => i.MethodName == "ExpectedException");

            Assert.AreEqual(info.ExpectedException, info.TestException);
            Assert.IsTrue(info.IsPassed);
        }

        [TestMethod]
        public void FailExceptionTest()
        {
            var info = regularTestsResults.Find(i => i.MethodName == "FailException");

            Assert.AreEqual(null, info.ExpectedException);
            Assert.AreNotEqual(null, info.TestException);
            Assert.IsFalse(info.IsPassed);
        }

        [TestMethod]
        public void UnexpectedExceptionTest()
        {
            var info = regularTestsResults.Find(i => i.MethodName == "UnexpectedException");

            Assert.AreNotEqual(info.TestException, info.ExpectedException);
            Assert.IsFalse(info.IsPassed);
        }

        //[TestMethod]
        //public void BeforeAttributeTest()
        //{
        //    MyNUnit.RunTests("..\\..\\..\\TestProjects\\BeforeTest\\Assembly");
        //    Assert.AreEqual(3, BeforeTest.BeforeClassTests.TestValue);
        //    Assert.AreEqual(4, BeforeTest.BeforeTests.TestValue);
        //}

        [TestMethod]
        public void AfterAttributeTest()
        {
            MyNUnit.RunTests("..\\..\\..\\TestProjects\\AfterTest\\Assembly");
            Assert.AreEqual(3, AfterTest.AfterClassTests.TestValue);
            Assert.AreEqual(4, AfterTest.AfterTests.TestValue);
        }
    }
}