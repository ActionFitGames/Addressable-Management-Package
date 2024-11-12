
using System;

namespace ActionFit.Framework.Addressable
{
    public class ProvideException : Exception
    {
        public ProvideException(string message) : base(message) { }
        public ProvideException(string message, Exception inner) : base(message, inner) { }
    }
}