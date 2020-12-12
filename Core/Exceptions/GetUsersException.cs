using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class GetUsersException : Exception
    {
        public GetUsersException(string message)
        : base(message)
        { }
    }
}
