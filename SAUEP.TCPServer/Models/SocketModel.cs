using System.Net;
using System.Net.Sockets;
using SAUEP.TCPServer.Interfaces;

namespace SAUEP.TCPServer.Models
{
    public sealed class SocketModel : IModel
    {
        #region Constructors

        public SocketModel()
        {
            ListenSocket = new TcpListener(IPAddress.Parse(_ipAddress), _listenPort);
            SaySocket = new TcpClient(_ipAddress, _sayPort);
            ListenSocket.Start();
        }

        #endregion



        #region Main Logic

        public TcpClient GetListenSocket()
        {
            return ListenSocket.AcceptTcpClient();
        }
        public TcpClient GetSaySocket()
        {
            return SaySocket;
        }

        #endregion



        #region Destructors

        ~SocketModel()
        {
            if (ListenSocket != null)
                ListenSocket.Stop();
            if (SaySocket != null)
                SaySocket.Close();
        }

        #endregion



        #region Fields

        private const int _listenPort = 8005;
        private const int _sayPort = 8006;
        private string _ipAddress = "127.0.0.1";
        public TcpListener ListenSocket { get; private set; }
        public TcpClient SaySocket { get; private set; }

        #endregion
    }
}
