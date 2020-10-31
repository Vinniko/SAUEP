using System;

namespace SAUEP.DeviceClient.Exceptions
{
    public sealed class DataBaseNullSelectException : Exception
    {
        public DataBaseNullSelectException(string message)
        : base(message)
        { }
    }
}
