using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class DeviceGroupModelDublicateException : Exception
    {
        public DeviceGroupModelDublicateException(string message)
        : base(message)
        { }
    }
}
