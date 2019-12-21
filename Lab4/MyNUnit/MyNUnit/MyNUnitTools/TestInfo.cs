using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit
{
    public class TestInfo
    {
        public string MethodName { get; private set; }
        public bool IsPassed { get; private set; }
        public Type ExpectedException { get; private set; }
        public Type TestException { get; private set; }

        public bool IsIgnored { get; private set; } = false;

        public string IgnoranceMessage { get; private set; } = "";

        public TestInfo(string name, bool isIgnored, string ignoranceMessage)
        {
            MethodName = name;
            IsIgnored = true;
            IgnoranceMessage = ignoranceMessage;
        }

        public TestInfo(string name, bool isPassed, Type expectedException, Type thrownException) 
        {
            MethodName = name;
            IsPassed = isPassed;
            ExpectedException = expectedException;
            TestException = thrownException;
        }
    }
}
