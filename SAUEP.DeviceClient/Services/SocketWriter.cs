using SAUEP.DeviceClient.Exceptions;
using SAUEP.DeviceClient.Interfaces;
using SAUEP.DeviceClient.Models;
using System;
using System.IO;

namespace SAUEP.DeviceClient.Services
{
    public sealed class SocketWriter : IWriter, IObserver
    {
        #region Constructors

        public SocketWriter(IParser parser, IWriter writer, ILoger logger)
        {
            _parser = parser;
            _consoleWriter = writer;
            _logger = logger;
        }

        #endregion



        #region Main Logic

        public void Write<T>(T data)
        {
            try
            {
                if (tcpSocketCreated() && Socket.SaySocket.Connected)
                {
                    using (var client = Socket.SaySocket)
                    {
                        using (var stream = client.GetStream())
                        {
                            using (var writer = new BinaryWriter(stream))
                            {
                                writer.Write(_parser.Depars(data as PollModel));
                                writer.Flush();
                                _consoleWriter.Write("Отправлено на сервер: " + (data as PollModel).Serial);
                                _logger.Logg("Отправлено на сервер: " + (data as PollModel).Serial);
                            }
                        }
                    }
                }
                else
                {
                    throw new TcpSocketIsClosedException();
                }
            }
            catch (TcpSocketIsClosedException ex)
            {
                _consoleWriter.Write("Нет доступа к серверу. Данные не отправлены." + (data as PollModel).Serial);
                _logger.Logg("Нет доступа к серверу. Данные не отправлены." + (data as PollModel).Serial);
                Socket.Reconnection();

                //TODO Кеширование данных
            }

            
               
            
        }

        public void Update(PollModel pollModel)
        {
            Write(pollModel);
        }

        #endregion



        #region Staff

        private bool tcpSocketCreated()
        {
            if (Socket.SaySocket == null)
            {
                throw new TcpSocketIsClosedException();
            }

            return true;
        }

        #endregion



        #region Fields

        public SocketModel Socket { get; set; }
        private IParser _parser;
        private IWriter _consoleWriter;
        private ILoger _logger;

        #endregion
    }
}
