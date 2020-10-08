using SAUEP.TCPServer.Interfaces;
using SAUEP.TCPServer.Models;
using System.IO;

namespace SAUEP.TCPServer.Services
{
    public sealed class SocketService : IListener
    {
        #region Constructors
        public SocketService(IWriter consoleWriter, IParser jsonParser, ILogger logger, IRepository repository)
        {
            _consoleWriter = consoleWriter;
            _jsonParser = jsonParser;
            _logger = logger;
            _repository = repository;
        }

        #endregion



        #region Main Logic

        public void Listen()
        {
            _consoleWriter.Write("Сервер запущен. Ожидание подключений...");
            _logger.Logg("Сервер запущен. Ожидание подключений...");
            while (true)
            {
                using (var client = Socket.GetSocket())
                {
                    using (var stream = client.GetStream())
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            PollModel model = _jsonParser.Pars<PollModel>(reader.ReadString());
                            _consoleWriter.Write("Получено: " + model.Id);
                            _logger.Logg("Получено: " + model.Id);
                            _repository.Set(model);
                            _consoleWriter.Write("Записано в базу данных: " + model.Id);
                            _logger.Logg("Записано в базу данных: " + model.Id);
                        }
                    }
                }
            }
        }

        #endregion



        #region Fields

        public SocketModel Socket { private get; set; }
        private IWriter _consoleWriter;
        private IParser _jsonParser;
        private ILogger _logger;
        private IRepository _repository;

        #endregion
    }
}
