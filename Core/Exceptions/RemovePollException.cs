using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class RemovePollException : Exception
    {
        public RemovePollException(string message)
            : base(message)
        { }
    }
}
