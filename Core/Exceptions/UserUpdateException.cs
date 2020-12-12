using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class UserUpdateException : Exception
    {
        public UserUpdateException(string message)
        : base(message)
        { }
    }
}
