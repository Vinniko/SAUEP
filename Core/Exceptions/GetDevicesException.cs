using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class GetDevicesException : Exception
    {
        public GetDevicesException(string message)
            : base(message)
            { }
    }
}
