using System;
using SAUEP.ApiServer.Interfaces;
using System.IO;

namespace SAUEP.ApiServer.Services
{
    public sealed class Logger : ILogger
    {
        #region Constructors

        public Logger()
        {
            _loggDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Log");
            _lock = new object();
        }

        #endregion



        #region Main Logic

        public void Logg(string text)
        {
            lock (_lock)
            {
                if (!Directory.Exists(_loggDirectory))
                    Directory.CreateDirectory(_loggDirectory);
                using (var streamWriter = File.AppendText(_loggDirectory + _loggFile))
                {
                    streamWriter.WriteLine(text + " : " + DateTime.Now.ToString());
                }
            }
        }

        #endregion



        #region Fields

        private string _loggDirectory;
        private const string _loggFile = "\\log.txt";
        private object _lock;

        #endregion
    }
}
