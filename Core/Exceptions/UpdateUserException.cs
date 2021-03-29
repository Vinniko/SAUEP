using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class UpdateUserException : Exception
    {
        public UpdateUserException(string message)
            : base(message)
        { }
    }
}
