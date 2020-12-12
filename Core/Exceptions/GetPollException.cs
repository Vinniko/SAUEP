using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class GetPollException : Exception
    {
        public GetPollException(string message)
            : base(message)
        { }
    }
}
