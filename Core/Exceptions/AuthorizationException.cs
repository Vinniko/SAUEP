using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class AuthorizationException : Exception
    {
        public AuthorizationException(string message)
        : base(message)
        { }
    }
}
