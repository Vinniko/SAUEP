using System.Collections.Generic;
using SAUEP.Core.Interfaces;
using SAUEP.Core.Models;
using System.IO;
using System;
using SAUEP.Core.Exceptions;
using System.Text;

namespace SAUEP.Core.Services
{
    public sealed class SocketListener : IListener
    {
        #region Constructors
        public SocketListener(IParser jsonParser, ILogger logger)
        {
            _jsonParser = jsonParser;
            _logger = logger;
        }

        #endregion



        #region Main Logic

        public IModel Listen()
        {
            _logger.Logg("Прослушивание сервера запущено. Ожидание данных...");
            //var client = Socket.ListenSocket;
            //var stream = client.GetStream();
            //byte[] buffer = new byte[256];
            //while (true)
            //{
            //    if (!Fill(stream, buffer, 4)) break;
            //    int len = BitConverter.ToInt32(buffer, 0);
            //    if (buffer.Length < len) buffer = new byte[len]; 
            //    if (!Fill(stream, buffer, len)) throw new EndOfStreamException();
            //    _pollModel = _jsonParser.Pars<PollModel>(Encoding.Unicode.GetString(buffer));
            //    _logger.Logg("Получен отчёт с устройства: " + _pollModel.Serial);
            //    return _pollModel;
            //}
            //throw new EmptyStreamException(_emptyStreamExceptionMessage);
            using (var client = Socket.ListenSocket)
            {
                using (var stream = client.GetStream())
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        _pollModel = _jsonParser.Pars<PollModel>(reader.ReadString());
                        _logger.Logg("Получен отчёт с устройства: " + _pollModel.Serial);
                        return _pollModel;
                    }
                }
            }
        }
        private bool Fill(Stream source, byte[] destination, int count)
        {
            int bytesRead, offset = 0;
            while (count > 0 &&
                (bytesRead = source.Read(destination, offset, count)) > 0)
            {
                offset += bytesRead;
                count -= bytesRead;
            }
            return count == 0;
        }

        #endregion



        #region Fields

        public SocketModel Socket { get; set; }
        private IParser _jsonParser;
        private ILogger _logger;
        private PollModel _pollModel;
        private const string _emptyStreamExceptionMessage = "Данных в потоке не было";

        #endregion
    }
}
