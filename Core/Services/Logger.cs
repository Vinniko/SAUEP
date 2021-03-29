using System;
using SAUEP.Core.Interfaces;
using System.IO;

namespace SAUEP.Core.Services
{
    public sealed class Logger : ILogger
    {

        #region Main Logic

        public void Logg(string text)
        {
            if (!Directory.Exists(_loggDirectory))
                Directory.CreateDirectory(_loggDirectory);
            lock (_lock)
                using (var streamWriter = File.AppendText(_loggDirectory + _loggFile))
                {
                    streamWriter.WriteLine(text + " : " + DateTime.Now.ToString());
                }
        }

        #endregion



        #region Fields

        private string _loggDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Log");
        private string _loggFile = "\\log.txt";
        private object _lock = new object();

        #endregion
    }
}
