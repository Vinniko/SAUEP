using System;

namespace SAUEP.ApiServer.Exceptions
{
    public sealed class DataBaseNullSelectException : Exception
    {
        public DataBaseNullSelectException(string message)
        : base(message)
        { }
    }
}
