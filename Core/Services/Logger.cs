using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using System.IO;

namespace Core.Services
{
    public sealed class Logger : ILogger
    {

        #region Main Logic

        public void Logg(string text)
        {
            if (!Directory.Exists(_loggDirectory))
                Directory.CreateDirectory(_loggDirectory);
            using (var streamWriter = File.CreateText(_loggDirectory + _loggFile))
            {
                streamWriter.WriteLine(text);
            }
        }

        #endregion



        #region Fields

        private string _loggDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Logggering");
        private string _loggFile = "\\log.txt";

        #endregion
    }
}
