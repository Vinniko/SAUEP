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
                var socket = guardian.Secure(() => 
                    new SocketModel()).Value;
                (listener as SocketListener).Socket = socket;
                guardian.Secure(() => 
                    (listener as SocketListener).AddObserver(scope.Resolve<SocketWriter>()));
                var listen = guardian.Secure(() => 
                    new Thread(new ThreadStart(listener.Listen))).Value;
                guardian.Secure(() => 
                    listen.Start());
                guardian.Secure(() => 
                    listen.Join());
            }
        }
    }

}
