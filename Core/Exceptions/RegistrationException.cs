using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class RegistrationException : Exception
    {
        public RegistrationException(string message)
       : base(message)
        { }
    }
}
