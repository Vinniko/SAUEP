using System.Net;
using System.Net.Sockets;
using SAUEP.DeviceClient.Interfaces;

namespace SAUEP.DeviceClient.Models
{
    public sealed class SocketModel : IModel
    {
        #region Constructors

        public SocketModel()
        {
            SaySocket = new TcpClient(_ipAddress, _sayPort);
        }

        #endregion



        #region Destructors

        ~SocketModel()
        {
            if (SaySocket != null)
                SaySocket.Close();
        }

        #endregion



        #region Fields

        private const int _sayPort = 8005;
        private string _ipAddress = "127.0.0.1";
        public TcpClient SaySocket { get; private set; }

        #endregion
    }
}
