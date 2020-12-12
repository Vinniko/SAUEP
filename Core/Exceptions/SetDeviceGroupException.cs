using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class SetDeviceGroupException : Exception
    {
        public SetDeviceGroupException(string message)
        : base(message)
        { }
    }
}
