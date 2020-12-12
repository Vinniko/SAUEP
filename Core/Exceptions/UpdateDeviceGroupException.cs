using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class UpdateDeviceGroupException : Exception
    {
        public UpdateDeviceGroupException(string message)
            : base(message)
        { }
    }
}
