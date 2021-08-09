
using System;

namespace SAUEP.DeviceClient.Exceptions
{
    public class TcpSocketIsClosedException : Exception
    {
        public TcpSocketIsClosedException(string message = _message) 
            : base(message)
        { }

        private const string _message = "TCP socket закрыт, подключение не удалось";
    }
}
