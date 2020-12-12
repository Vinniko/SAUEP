using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class UserRemoveException : Exception
    {
        public UserRemoveException(string message)
        : base(message)
        { }
    }
}
