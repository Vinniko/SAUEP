using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class GetDeviceGroupException : Exception
    {
        public GetDeviceGroupException(string message)
            : base(message)
        { }
    }
}
