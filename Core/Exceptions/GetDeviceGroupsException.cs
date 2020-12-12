using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class GetDeviceGroupsException : Exception
    {
        public GetDeviceGroupsException(string message)
            : base(message)
        { }
    }
}
