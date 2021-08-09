using System.Net.Sockets;
using SAUEP.DeviceClient.Interfaces;

namespace SAUEP.DeviceClient.Models
{
    public sealed class SocketModel : IModel
    {
        #region Constructors

        public SocketModel(string ipAddress, int sayPort)
        {
            _sayHost = ipAddress;
            _sayPort = sayPort;

            try
            {
                SaySocket = new TcpClient(ipAddress, sayPort);
            }
            catch(SocketException e)
            {
                SaySocket = null;
            }
        }

        #endregion



        #region Main Logic

        public void Reconnection()
        {
            try
            {
                SaySocket = new TcpClient(_sayHost, _sayPort);
            }
            catch (SocketException e)
            {
                SaySocket = null;
            }
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

        public TcpClient SaySocket { get; private set; }
        private string _sayHost;
        private int _sayPort;

        #endregion
    }
}
