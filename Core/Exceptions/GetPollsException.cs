using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class GetPollsException : Exception
    {
        public GetPollsException(string message) 
            : base(message)
        { }
    }
}
