using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class UpdateDeviceException : Exception
    {
        public UpdateDeviceException(string message)
            : base(message)
            { }
}
}
