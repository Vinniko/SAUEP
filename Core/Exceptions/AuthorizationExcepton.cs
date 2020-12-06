using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class AuthorizationExcepton : Exception
    {
        public AuthorizationExcepton(string message)
        : base(message)
        { }
    }
}
