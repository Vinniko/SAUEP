using SAUEP.Core.Interfaces;
using SAUEP.Core.Models;

namespace SAUEP.Core.Connections
{
    public class ServerConnection : IModel, IConnection
    {
        #region Constructors

        public ServerConnection(IReader fileReader, IParser jsonParser)
        {
            _fileReader = fileReader;
            _jsonParser = jsonParser;
            ConnectionUrl = _jsonParser.Pars<ConnectionModel>(_fileReader.Read(_connectionFile)).ConnectionUrl;
        }

        #endregion



        #region Fields

        public string ConnectionUrl { get; private set; }
        private IReader _fileReader;
        private IParser _jsonParser;
        private string _connectionFile = @"Connections\Connection.json";

        #endregion
    }
}
