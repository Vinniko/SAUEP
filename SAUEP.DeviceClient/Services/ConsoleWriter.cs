using System;
using SAUEP.DeviceClient.Interfaces;


namespace SAUEP.DeviceClient.Services
{
    public sealed class ConsoleWriter : IWriter
    {
        public void Write<T>(T data)
        {
            Console.WriteLine(data.ToString());
        }
    }
}
