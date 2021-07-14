using System.Net.Sockets;
using SAUEP.DeviceClient.Interfaces;

namespace SAUEP.DeviceClient.Models
{
    public sealed class SocketModel : IModel
    {
        #region Constructors

        public SocketModel(string ipAddress, int sayPort)
        {
            SaySocket = new TcpClient(ipAddress, sayPort);
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

        #endregion
    }
}
