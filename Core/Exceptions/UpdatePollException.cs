using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class UpdatePollException : Exception
    {
        public UpdatePollException(string message)
            : base(message)
        { }
    }
}
