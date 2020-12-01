using System.Net;
using System.Net.Sockets;
using SAUEP.Core.Interfaces;

namespace SAUEP.Core.Models
{
    public sealed class SocketModel : IModel
    {
        #region Constructors

        public SocketModel()
        {
            ListenSocket = new TcpListener(IPAddress.Parse(_ipAddress), _listenPort);
            ListenSocket.Start();
        }

        #endregion



        #region Main Logic

        public TcpClient GetListenSocket()
        {
            return ListenSocket.AcceptTcpClient();
        }
        #endregion



        #region Destructors

        ~SocketModel()
        {
            if (ListenSocket != null)
                ListenSocket.Stop();
        }

        #endregion



        #region Fields

        private const int _listenPort = 8003;
        private string _ipAddress = "127.0.0.1";
        public TcpListener ListenSocket { get; private set; }

        #endregion
    }
}
