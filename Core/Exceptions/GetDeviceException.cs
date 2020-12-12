using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class GetDeviceException : Exception
    {
        public GetDeviceException(string message)
            : base(message)
            { }
}
}
