using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class RemoveDeviceException : Exception
    {
        public RemoveDeviceException(string message)
            : base(message)
            { }
}
}
