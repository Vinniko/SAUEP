using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class BadKeyException : Exception
    {
        public BadKeyException(string message)
            : base(message) 
        { }
    }
}
