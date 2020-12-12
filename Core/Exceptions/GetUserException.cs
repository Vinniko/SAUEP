using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class GetUserException : Exception
    {
        public GetUserException(string message)
        : base(message)
        { }
    }
}
