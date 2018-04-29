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
            StartServerAsync();
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
                        int bytes = 0;
                        byte[] data = new byte[512];
                        StringBuilder message = new StringBuilder();

                        do
                        {
                            bytes = await stream.ReadAsync(data, 0, data.Length);
                            message.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (stream.DataAvailable);

                        Console.WriteLine($"Получено сообщение: {message.ToString()}");

                        data = Encoding.Unicode.GetBytes(message.ToString().Reverse().ToArray());
                        WriteStreamAsync(stream, data);
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

        static async Task WriteStreamAsync(NetworkStream stream, byte[] message)
        {
            try
            {
                await stream.WriteAsync(message, 0, message.Length);
                Console.WriteLine("Ответ отправлен");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (stream != null)
                    stream.Dispose();
            }
        }
    }
}
