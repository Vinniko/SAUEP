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
            ListenSocket = new TcpClient(_ipAddress, _listenPort);
        }

        #endregion



        #region Destructors

        ~SocketModel()
        {
            if (ListenSocket != null)
                ListenSocket.Close();
        }

        #endregion



        #region Fields

        private const int _listenPort = 8003;
        private string _ipAddress = "127.0.0.1";
        public TcpClient ListenSocket { get; private set; }

        #endregion
    }
}
