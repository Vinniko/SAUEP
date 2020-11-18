using SAUEP.ApiServer.Interfaces;
using SAUEP.ApiServer.Models;
using Npgsql;

namespace SAUEP.ApiServer.Connections
{
    public sealed class DataBaseConnection : IConnection
    {
        #region Constructors

        public DataBaseConnection(IReader reader, IParser jsonParser)
        {
            _reader = reader;
            _parser = jsonParser;
            var connection = _parser.Pars<ConnectionModel>(_reader.Read(_connectionFile));
            Connection = new NpgsqlConnection(string.Format(_connection, (connection as ConnectionModel).Host, (connection as ConnectionModel).Username, (connection as ConnectionModel).Password, (connection as ConnectionModel).Database));
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

        private const string _connection = "Host={0};Username={1};Password={2};Database={3}";
        private const string _connectionFile = @"Connections\Connection.json";
        private IReader _reader;
        private IParser _parser;
        public NpgsqlConnection Connection { get; set; }

        #endregion
    }
}
