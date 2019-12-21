using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNUnit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit.Tests
{
    [TestClass]
    public class MyNUnitTests
    {
        [TestMethod]
        public void DifferentTestAttributeScenarioTest()
        {
            var path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;

            path = Path.Combine(path, "MyNUnit\\TestProjects\\TestResult\\bin");
        }

        [TestMethod]
        public void BeforeAttrubuteTest()
        {

        }

        [TestMethod]
        public void AfterAttributeTest()
        {

        }
    }
}