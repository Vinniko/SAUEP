using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using SAUEP.DeviceClient.Services;
using SAUEP.DeviceClient.Configs;
using Autofac;

namespace SAUEP.DeviceClient
{
    class Program
    {
        const int PORT = 8005;
        const string ADDRESS = "127.0.0.1";
        static void Main(string[] args)
        {
            IContainer container = AutofacConfig.ConfigureContainer();
            Guardian guardian = container.Resolve<Guardian>();
            ResultGenerator generator = container.Resolve<ResultGenerator>();
            guardian.Secure(generator.Generate);
            //Thread myThread = new Thread(new ThreadStart(work));
            //Thread myThread1 = new Thread(new ThreadStart(work));
            //Thread myThread2 = new Thread(new ThreadStart(work));
            //myThread.Start(); // запускаем поток
            //myThread1.Start();
            //myThread2.Start();

            //for (int i = 1; i < 9; i++)
            //{
            //    work();
            //}

            //Console.ReadLine();
        }
        public static void work()
        {
            TcpClient client = null;
            try
            {
                client = new TcpClient(ADDRESS, PORT);
                Console.Write("Введите сообщение: ");
                string message = "kek";
                NetworkStream stream = client.GetStream();

                // отправляем сообщение
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(message);
                writer.Flush();


                writer.Close();
                stream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (client != null)
                    client.Close();
            }
        }
    }
}
