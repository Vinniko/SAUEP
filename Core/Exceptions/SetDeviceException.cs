using System;

namespace SAUEP.Core.Exceptions
{
    public sealed class SetDeviceException : Exception
    {
        public SetDeviceException(string message)
            : base(message)
        { }
    }
}
