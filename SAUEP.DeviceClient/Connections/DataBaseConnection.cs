using SAUEP.TCPServer.Interfaces;
using Npgsql;

namespace SAUEP.DeviceClient.Connections
{
    public sealed class DataBaseConnection : IConnection
    {
        #region Constructors

        public DataBaseConnection()
        {
            Connection = new NpgsqlConnection(_connection);
            Connection.Open();
        }

        #endregion



        #region Destructors
        ~DataBaseConnection()
        {
            Connection.Close();
        }

        #endregion



        #region Fields

        private const string _connection = "Host=localhost;Username=postgres;Password=123456789q;Database=SAUEPOLTP";
        public NpgsqlConnection Connection { get; set; }

        #endregion
    }
}
