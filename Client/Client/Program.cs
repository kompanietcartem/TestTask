using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private const string address = "127.0.0.1";
        private const int port = 1234;

        static void Main(string[] args)
        {
            while (true)
            {
                Task t = StartClient();
                t.Wait();
                t.Dispose();
            }
        }

        static async Task StartClient()
        {
            TcpClient client = null;
            try
            {
                NetworkStream stream = await ConnectToServer(client);
                Task read = ReadStream(stream);
                Task write = WriteStream(stream);
                write.Wait();
                read.Wait();
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

        static async Task<NetworkStream> ConnectToServer(TcpClient client)
        {
            try
            {
                Console.WriteLine("Подключение к серверу...");
                client = new TcpClient();
                await client.ConnectAsync(address, port);
                Console.WriteLine("Подключено");
                return client.GetStream();
            }
            catch
            {
                Thread.Sleep(1000);
                return await ConnectToServer(client);
            }
        }

        static async Task WriteStream(NetworkStream stream)
        {
            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);
                Console.WriteLine("Сообщение отправлено");
            }
        }

        static async Task ReadStream(NetworkStream stream)
        {
            while (true)
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
                Console.WriteLine($"Сообщение от сервера: {message.ToString()}");
            }
        }
    }
}
