﻿using System;
using SAUEP.DeviceClient.Interfaces;
using System.IO;

namespace SAUEP.DeviceClient.Services
{
    public sealed class Loger : ILoger
    {

        #region Main Logic

        public void Logg(string text)
        {
            if (!Directory.Exists(_loggDirectory))
            {
                Directory.CreateDirectory(_loggDirectory);
            }

            using (var streamWriter = File.AppendText(_loggDirectory + _loggFile))
            {
                streamWriter.WriteLine(text + " : " + DateTime.Now.ToString());
            }
        }

        #endregion



        #region Fields

        private string _loggDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Log");
        private const string _loggFile = "\\log.txt";

        #endregion
    }
}
