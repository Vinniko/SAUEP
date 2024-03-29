﻿using System;
using SAUEP.TCPServer.Interfaces;
using System.IO;

namespace SAUEP.TCPServer.Services
{
    public sealed class Logger : ILogger
    {

        #region Main Logic

        public void Logg(string text)
        {
            if (!Directory.Exists(_loggDirectory))
                Directory.CreateDirectory(_loggDirectory);
            using (var streamWriter = File.AppendText(_loggDirectory + _loggFile))
            {
                streamWriter.WriteLine(text + " : " + DateTime.Now.ToString());
            }
        }

        #endregion



        #region Fields

        private string _loggDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Log");
        private string _loggFile = "\\log.txt";

        #endregion
    }
}
