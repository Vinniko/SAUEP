using System;
using SAUEP.Core.Interfaces;
using System.IO;

namespace SAUEP.Core.Services
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
