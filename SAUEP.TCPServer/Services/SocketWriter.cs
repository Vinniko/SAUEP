using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAUEP.TCPServer.Interfaces;
using SAUEP.TCPServer.Models;
using SAUEP.TCPServer.Services;
using System.IO;

namespace SAUEP.TCPServer.Services
{
    public sealed class SocketWriter : IWriter, IObserver
    {
        #region Constructors

        public SocketWriter(IParser parser, ConsoleWriter writer, ILogger logger)
        {
            _parser = parser;
            _consoleWriter = writer;
            _logger = logger;
        }

        #endregion



        #region Main Logic

        public void Write<T>(T data)
        {
            using (var client = Socket.GetSaySocket())
            {
                using (var stream = client.GetStream())
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(_parser.Depars<PollModel>(data as PollModel));
                        writer.Flush();
                        _consoleWriter.Write("Отправлено с сервера: " + (data as PollModel).Id);
                        _logger.Logg("Отправлено с сервера: " + (data as PollModel).Id);
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
