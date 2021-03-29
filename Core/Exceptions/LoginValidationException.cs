using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class LoginValidationException : Exception
    {
        public LoginValidationException(string message) 
            : base(message)
        { }
    }
}
