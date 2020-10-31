using SAUEP.DeviceClient.Interfaces;
using SAUEP.DeviceClient.Models;
using System.IO;

namespace SAUEP.DeviceClient.Services
{
    public sealed class SocketWriter : IWriter, IObserver
    {
        #region Constructors

        public SocketWriter(IParser parser, IWriter writer, ILogger logger)
        {
            _parser = parser;
            _consoleWriter = writer;
            _logger = logger;
        }

        #endregion



        #region Main Logic

        public void Write<T>(T data)
        {
            using (var client = Socket.SaySocket)
            {
                using (var stream = client.GetStream())
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(_parser.Depars<PollModel>(data as PollModel));
                        writer.Flush();
                        _consoleWriter.Write("Отправлено на сервер: " + (data as PollModel).Serial);
                        _logger.Logg("Отправлено на сервер: " + (data as PollModel).Serial);
                    }
                }
            }
        }

        public void Update(PollModel pollModel)
        {
            Write(pollModel);
        }

        #endregion



        #region Fields

        public SocketModel Socket { get; set; }
        private IParser _parser;
        private IWriter _consoleWriter;
        private ILogger _logger;

        #endregion
    }
}
