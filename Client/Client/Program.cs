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
            Task t = StartClient();
            t.Wait();
        }

        static async Task StartClient()
        {
            TcpClient client = null;
            try
            {
                NetworkStream stream = await ConnectToServer(client);
                await WorkWithServer(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                StartClient();
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

        static async Task WorkWithServer(NetworkStream stream)
        {
            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);

                data = new byte[data.Length];
                string response;
                do
                {
                    await stream.ReadAsync(data, 0, data.Length);
                    response = Encoding.Unicode.GetString(data);
                }
                while (stream.DataAvailable);
                Console.WriteLine($"Ответ сервера: {response}");
            }
        }
    }
}
