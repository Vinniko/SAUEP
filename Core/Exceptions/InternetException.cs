using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class InternetException : Exception
    {
        public InternetException(string message)
            : base(message)
        { }
    }
}
