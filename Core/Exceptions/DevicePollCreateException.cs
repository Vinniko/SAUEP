using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class DevicePollCreateException : Exception
    {
        public DevicePollCreateException(string message)
            : base(message)
        { }
    }
}
