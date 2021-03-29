using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class UserCreateException : Exception
    {
        public UserCreateException(string message) 
            : base(message)
        { }
    }
}
