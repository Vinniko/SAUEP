using System;
using SAUEP.DeviceClient.Interfaces;
using System.IO;

namespace SAUEP.DeviceClient.Services
{
    public sealed class Loger : ILoger
    {

        #region Main Logic

        public void Log(string text)
        {
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
            using (var streamWriter = File.AppendText(_logDirectory + _logFile))
            {
                streamWriter.WriteLine(text + " : " + DateTime.Now.ToString());
            }
        }

        #endregion




        #region Fields

        private string _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Log");
        private string _logFile = "\\log.txt";

        #endregion
    }
}
