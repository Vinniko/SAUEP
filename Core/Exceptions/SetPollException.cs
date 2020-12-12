using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class SetPollException : Exception
    {
        public SetPollException(string message)
            : base(message)
        { }
    }
}
