using System;

namespace MyThreadPool
{
    /// <summary>
    /// Исключение, бросаемое при возникновении исключения в цепочке заданий из ContinueWith
    /// </summary>
    [Serializable]
    public class ContinueWithChainException : Exception
    {
        public ContinueWithChainException() : base() { }

        public ContinueWithChainException(string message) : base(message) { }
    }
}
