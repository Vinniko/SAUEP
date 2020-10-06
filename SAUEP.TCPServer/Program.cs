using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Autofac;
using SAUEP.TCPServer.Configs;
using SAUEP.TCPServer.Services;
using SAUEP.TCPServer.Interfaces;

namespace SAUEP.TCPServer
{
    public class Program
    {
        public static void test()
        {
            int count = 0;
            throw new Exception();
        }
        static int port = 8005; // порт для приема входящих запросов
        public static void Main(string[] args)
        {
            IContainer container = AutofacConfig.ConfigureContainer();
            using (var scope = container.BeginLifetimeScope())
            {
                var guardian = scope.Resolve<Guardian>();
                var k = scope.Resolve<ILogger>();
                guardian.Secure(() => test());
            }

            
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);

                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());

                    // отправляем ответ
                    string message = "ваше сообщение доставлено";
                    data = Encoding.Unicode.GetBytes(message);
                    handler.Send(data);
                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
