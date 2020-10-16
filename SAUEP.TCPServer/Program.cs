using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Autofac;
using SAUEP.TCPServer.Configs;
using SAUEP.TCPServer.Services;
using SAUEP.TCPServer.Interfaces;
using SAUEP.TCPServer.Models;
using System.IO;
using System.Threading;

namespace SAUEP.TCPServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IContainer container = AutofacConfig.ConfigureContainer();
            using (var scope = container.BeginLifetimeScope())
            {
                var guardian = scope.Resolve<Guardian>();
                var listener = scope.Resolve<IListener>();
                var socket = new SocketModel();
                (listener as SocketListener).Socket = socket;
                (listener as SocketListener).AddObserver(scope.Resolve<SocketWriter>());
                var listen = new Thread(new ThreadStart(listener.Listen));
                guardian.Secure(listen.Start);
                listen.Join();
            }
        }
    }

}
