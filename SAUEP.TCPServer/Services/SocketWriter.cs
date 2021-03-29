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
            if (Socket.SaySocket.Pending())
            {
                using (var client = Socket.GetSaySocket())
                {
                    using (var stream = client.GetStream())
                    {
                        //byte[] bytes = Encoding.Unicode.GetBytes(_parser.Depars(data as PollModel));
                        //byte[] count = BitConverter.GetBytes(bytes.Length);
                        //stream.Write(count, 0, count.Length);
                        //stream.Flush();
                        //stream.Write(bytes, 0, bytes.Length);
                        //stream.Flush();
                        //_consoleWriter.Write("Отправлено с сервера: " + (data as PollModel).Serial);
                        //_logger.Logg("Отправлено с сервера: " + (data as PollModel).Serial);
                        using (var writer = new BinaryWriter(stream))
                        {
                            try
                            {
                                writer.Write(_parser.Depars<PollModel>(data as PollModel));
                                writer.Flush();
                                _consoleWriter.Write("Отправлено с сервера: " + (data as PollModel).Serial);
                                _logger.Logg("Отправлено с сервера: " + (data as PollModel).Serial);
                            }
                            catch(IOException ex)
                            {
                                _logger.Logg("Потерянна связь с клиентом " + ex.Message);
                            }
                           
                        }
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
