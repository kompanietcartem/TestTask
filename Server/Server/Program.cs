using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        private const int port = 1234;

        static void Main(string[] args)
        {
            Task t = StartServerAsync();
            Console.ReadLine();
        }

        static async Task StartServerAsync()
        {
            TcpListener server = null;
            try
            {
                server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                server.Start();
                Console.WriteLine("Сервер запущен");

                while (true)
                {
                    Console.WriteLine("Ожидание нового клиента...");
                    TcpClient client = await server.AcceptTcpClientAsync();
                    Console.WriteLine("Клиент подключен");
                    WorkWithClientAsync(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (server != null)
                    server.Stop();
            }

        }

        static async Task WorkWithClientAsync(TcpClient client)
        {
            NetworkStream stream = null;
            try
            {
                using (stream = client.GetStream())
                {
                    while (client.Connected)
                    {
                        byte[] data = new byte[512];
                        await stream.ReadAsync(data, 0, data.Length);
                        string message = Encoding.Unicode.GetString(data).Trim(new char[] { '\0' });
                        Console.WriteLine($"Получено сообщение: {message}");

                        data = Encoding.Unicode.GetBytes(message.Reverse().ToArray());
                        await stream.WriteAsync(data, 0, data.Length);
                        Console.WriteLine("Ответ отправлен");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }
    }
}
