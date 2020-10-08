using System.Net;
using System.Net.Sockets;

namespace SAUEP.TCPServer.Models
{
    public sealed class SocketModel
    {
        #region Constructors

        public SocketModel()
        {
            ListenSocket = new TcpListener(IPAddress.Parse(_ipAddress), _port);
            ListenSocket.Start();
        }

        #endregion



        #region Main Logic

        public TcpClient GetSocket()
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

        private int _port = 8005;
        private string _ipAddress = "127.0.0.1";
        public TcpListener ListenSocket { get; private set; }

        #endregion
    }
}
