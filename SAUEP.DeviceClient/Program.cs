using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SAUEP.DeviceClient.Services;
using SAUEP.DeviceClient.Configs;
using Autofac;
using SAUEP.DeviceClient.Interfaces;
using SAUEP.DeviceClient.Models;

namespace SAUEP.DeviceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            IContainer container = AutofacConfig.ConfigureContainer();
            Guardian guardian = container.Resolve<Guardian>();
            IWriter socketWriter = guardian.Secure(()=>container.Resolve<SocketWriter>()).Value;
            ResultGenerator generator = guardian.Secure(()=>container.Resolve<ResultGenerator>()).Value;
            guardian.Secure(() => generator.AddObserver(socketWriter as SocketWriter));
            IRepository deviceRepository = guardian.Secure(() => container.Resolve<IRepository>()).Value;
            while (true)
            {
                IModel socket = guardian.Secure(() => new SocketModel()).Value;
                (socketWriter as SocketWriter).Socket = socket as SocketModel;
                ICollection<Thread> threads = new List<Thread>();
                foreach (var device in deviceRepository.Get<DeviceModel>())
                {
                    threads.Add(new Thread(new ParameterizedThreadStart(generator.Generate)));
                    threads.ElementAt(threads.Count - 1).Start(device);
                }
                foreach (var thread in threads)
                {
                    thread.Join();
                }
            }
        }
    }
}
