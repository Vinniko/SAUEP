using System.Collections.Generic;
using SAUEP.TCPServer.Interfaces;
using SAUEP.TCPServer.Models;
using System.IO;

namespace SAUEP.TCPServer.Services
{
    public sealed class SocketListener : IListener, IObservable
    {
        #region Constructors
        public SocketListener(IWriter consoleWriter, IParser jsonParser, ILogger logger, IRepository repository)
        {
            _consoleWriter = consoleWriter;
            _jsonParser = jsonParser;
            _logger = logger;
            _repository = repository;
            observers = new List<IObserver>();
        }

        #endregion



        #region Main Logic

        public void Listen()
        {
            _consoleWriter.Write("Сервер запущен. Ожидание подключений...");
            _logger.Logg("Сервер запущен. Ожидание подключений...");
            while (true)
            {
                using (var client = Socket.GetListenSocket())
                {
                    using (var stream = client.GetStream())
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            _pollModel = _jsonParser.Pars<PollModel>(reader.ReadString());
                            _consoleWriter.Write("Получено: " + _pollModel.Id);
                            _logger.Logg("Получено: " + _pollModel.Id);
                            _repository.Set(_pollModel);
                            _consoleWriter.Write("Записано в базу данных: " + _pollModel.Serial);
                            _logger.Logg("Записано в базу данных: " + _pollModel.Serial);
                            NotifyObservers();
                        }
                    }
                }
            }
        }

        public void AddObserver(IObserver observer)
        {
            observers.Add(observer);
        }

        public void NotifyObservers()
        {
            foreach (IObserver observer in observers)
                observer.Update(_pollModel);
        }

        public void RemoveObserver(IObserver observer)
        {
            observers.Remove(observer);
        }

        #endregion



        #region Fields

        public SocketModel Socket { private get; set; }
        private IWriter _consoleWriter;
        private IParser _jsonParser;
        private ILogger _logger;
        private IRepository _repository;
        private ICollection<IObserver> observers;
        private PollModel _pollModel;

        #endregion
    }
}
