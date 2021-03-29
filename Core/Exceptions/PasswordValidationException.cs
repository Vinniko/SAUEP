using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class PasswordValidationException : Exception
    {
        public PasswordValidationException(string message)
            : base(message)
        { }
    }
}
