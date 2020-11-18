using System;
using SAUEP.ApiServer.Interfaces;
using System.IO;

namespace SAUEP.ApiServer.Services
{
    public sealed class FileReader : IReader
    {
        public string Read(string path)
        {
            string text = string.Empty;
            using (StreamReader stream = File.OpenText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path)))
            {
                text = stream.ReadToEnd();
            }
            return text;
        }
    }
}
