using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

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
            var resultsTestPath = "..\\..\\..\\TestProjects\\TestResult\\Assembly";

            var regularTestsReport = MyNUnit.RunTestsAndGetReport(resultsTestPath);

            foreach (var list in regularTestsReport.Values)
            {
                foreach (var info in list)
                {
                    regularTestsResults.Add(info);
                }
            }

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
            var resultsTestPath = "..\\..\\..\\TestProjects\\TestResult\\Assembly";

            var regularTestsReport = MyNUnit.RunTestsAndGetReport(resultsTestPath);

            foreach (var list in regularTestsReport.Values)
            {
                foreach (var info in list)
                {
                    regularTestsResults.Add(info);
                }
            }

            var successInfo = regularTestsResults.Find(i => i.MethodName == "Success");

            Assert.IsTrue(successInfo.IsPassed);
        }

        [TestMethod]
        public void IgnoreTest()
        {
            var resultsTestPath = "..\\..\\..\\TestProjects\\TestResult\\Assembly";

            var regularTestsReport = MyNUnit.RunTestsAndGetReport(resultsTestPath);

            foreach (var list in regularTestsReport.Values)
            {
                foreach (var info in list)
                {
                    regularTestsResults.Add(info);
                }
            }

            var ignoreInfo = regularTestsResults.Find(i => i.MethodName == "Ignore");
            var exceptionIgnoreInfo = regularTestsResults.Find(i => i.MethodName == "IgnoreException");

            Assert.IsTrue(ignoreInfo.IsIgnored);
            Assert.AreEqual("Let's ignore this method", ignoreInfo.IgnoranceMessage);
            Assert.IsTrue(exceptionIgnoreInfo.IsIgnored);
        }

        [TestMethod]
        public void ExpectedExceptionTest()
        {
            var resultsTestPath = "..\\..\\..\\TestProjects\\TestResult\\Assembly";

            var regularTestsReport = MyNUnit.RunTestsAndGetReport(resultsTestPath);

            foreach (var list in regularTestsReport.Values)
            {
                foreach (var info in list)
                {
                    regularTestsResults.Add(info);
                }
            }

            var expectedInfo = regularTestsResults.Find(i => i.MethodName == "ExpectedException");

            Assert.AreEqual(expectedInfo.ExpectedException, expectedInfo.TestException);
            Assert.IsTrue(expectedInfo.IsPassed);
        }

        [TestMethod]
        public void FailExceptionTest()
        {
            var resultsTestPath = "..\\..\\..\\TestProjects\\TestResult\\Assembly";

            var regularTestsReport = MyNUnit.RunTestsAndGetReport(resultsTestPath);

            foreach (var list in regularTestsReport.Values)
            {
                foreach (var info in list)
                {
                    regularTestsResults.Add(info);
                }
            }

            var failInfo = regularTestsResults.Find(i => i.MethodName == "FailException");

            Assert.AreEqual(null, failInfo.ExpectedException);
            Assert.AreNotEqual(null, failInfo.TestException);
            Assert.IsFalse(failInfo.IsPassed);
        }

        [TestMethod]
        public void UnexpectedExceptionTest()
        {
            var resultsTestPath = "..\\..\\..\\TestProjects\\TestResult\\Assembly";

            var regularTestsReport = MyNUnit.RunTestsAndGetReport(resultsTestPath);

            foreach (var list in regularTestsReport.Values)
            {
                foreach (var info in list)
                {
                    regularTestsResults.Add(info);
                }
            }

            var exceptionInfo = regularTestsResults.Find(i => i.MethodName == "UnexpectedException");

            Assert.AreNotEqual(exceptionInfo.TestException, exceptionInfo.ExpectedException);
            Assert.IsFalse(exceptionInfo.IsPassed);
        }

        [TestMethod]
        public void AfterBeforeAttributeTest()
        {
            var reportAfter = MyNUnit.RunTestsAndGetReport("..\\..\\..\\TestProjects\\BeforeAfterTest\\Assembly");

            var testedMethodsCount = 0;

            foreach (var methodReports in reportAfter.Values)
            {
                foreach (var methodReport in methodReports)
                {
                    ++testedMethodsCount;
                }
            }

            Assert.AreEqual(2, testedMethodsCount);
        }
    }
}