using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyNUnit.Tests
{
    /// <summary>
    /// Тесты для MyNUnit
    /// </summary>
    [TestClass]
    public class MyNUnitTests
    {
        private string root;
        private List<TestInfo> regularTestsResults;
        private List<string> expectedRegularResultsMethods;
        private List<TestInfo> exceptionResults;

        [TestInitialize]
        public void Initialize()
        {
            expectedRegularResultsMethods = new List<string>();
            expectedRegularResultsMethods.Add("Success");
            expectedRegularResultsMethods.Add("Ignore");
            expectedRegularResultsMethods.Add("IgnoreException");
            expectedRegularResultsMethods.Add("ExpectedException");
            expectedRegularResultsMethods.Add("FailException");
            expectedRegularResultsMethods.Add("UnexpectedException");

            var resultsTestPath = "..\\..\\..\\TestProjects\\TestResult\\bin";

            var regularTestsReport = MyNUnit.RunTestsAndGetReport(resultsTestPath);

            regularTestsResults = regularTestsReport[typeof(TestResult.Tests)];
            exceptionResults = regularTestsReport[typeof(TestResult.ExceptionTests)];
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void MethodsFormatTest()
        {
            MyNUnit.RunTests("..\\..\\..\\TestProjects\\WrongFormatTest\\bin");
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void MethodsParametersFormatTest()
        {
            MyNUnit.RunTests("..\\..\\..\\TestProjects\\WrongParametersFormatTest\\bin");
        }

        [TestMethod]
        public void CorrectMethodsAreTestedTest()
        {
            var names = new List<string>();

            foreach (var res in regularTestsResults)
            {
                names.Add(res.MethodName);
            }

            foreach (var res in exceptionResults)
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
            var exceptionIgnoreInfo = exceptionResults.Find(i => i.MethodName == "IgnoreException");

            Assert.IsTrue(ignoreInfo.IsIgnored);
            Assert.AreEqual("Let's ignore this method", ignoreInfo.IgnoranceMessage);
            Assert.IsTrue(exceptionIgnoreInfo.IsIgnored);
        }

        [TestMethod]
        public void ExpectedExceptionTest()
        {
            var info = exceptionResults.Find(i => i.MethodName == "ExpectedException");

            Assert.AreEqual(info.ExpectedException, info.TestException);
            Assert.IsTrue(info.IsPassed);
        }

        [TestMethod]
        public void FailExceptionTest()
        {
            var info = exceptionResults.Find(i => i.MethodName == "FailException");

            Assert.AreEqual(null, info.ExpectedException);
            Assert.AreNotEqual(null, info.TestException);
            Assert.IsFalse(info.IsPassed);
        }

        [TestMethod]
        public void UnexpectedExceptionTest()
        {
            var info = exceptionResults.Find(i => i.MethodName == "UnexpectedException");

            Assert.AreNotEqual(info.TestException, info.ExpectedException);
            Assert.IsFalse(info.IsPassed);
        }

        [TestMethod]
        public void BeforeAttributeTest()
        {
            MyNUnit.RunTests("..\\..\\..\\TestProjects\\BeforeTest\\bin");
            Assert.AreEqual(3, BeforeTest.BeforeClassTests.TestValue);
            Assert.AreEqual(4, BeforeTest.BeforeTests.TestValue);
        }

        [TestMethod]
        public void AfterAttributeTest()
        {
            MyNUnit.RunTests("..\\..\\..\\TestProjects\\AfterTest\\bin");
            Assert.AreEqual(3, AfterTest.AfterClassTests.TestValue);
            Assert.AreEqual(4, AfterTest.AfterTests.TestValue);
        }
    }
}