using System;
using SAUEP.TCPServer.Interfaces;

namespace SAUEP.TCPServer.Services
{
    public sealed class ConsoleWriter : IWriter
    {
        public void Write<T>(T data)
        {
            Console.WriteLine(data.ToString());
        }
    }
}
