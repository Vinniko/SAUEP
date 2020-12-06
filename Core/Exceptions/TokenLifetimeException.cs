using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class TokenLifetimeException : Exception
    {
        public TokenLifetimeException(string message)
        : base(message)
        { }
    }
}
