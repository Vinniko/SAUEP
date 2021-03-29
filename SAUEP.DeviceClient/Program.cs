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
            object locker = new object();
            while (true)
            {
                ICollection<Thread> threads = new List<Thread>();
                foreach (var device in deviceRepository.Get<DeviceModel>())
                {
                    if (!device.Status) continue;
                    threads.Add(new Thread(new ThreadStart(() =>
                    {
                        lock (locker)
                        {
                            IModel socket = guardian.Secure(() => new SocketModel()).Value;
                            (socketWriter as SocketWriter).Socket = socket as SocketModel;
                            generator.Generate(device);
                        }
                    }
                    ))); 
                    threads.ElementAt(threads.Count - 1).Start();
                }
                foreach (var thread in threads)
                {
                    thread.Join();
                }
                Thread.Sleep(3000);
            }
        }
    }
}
