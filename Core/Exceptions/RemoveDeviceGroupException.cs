using System;

namespace SAUEP.Core.Exceptions
{
    public class RemoveDeviceGroupException : Exception
    {
        public RemoveDeviceGroupException(string message)
            : base(message)
        { }
    }
}
