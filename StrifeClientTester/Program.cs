using System;
using System.Net;
using System.Net.Sockets;

namespace IronStrifeMasterServer
{
    class ClientTester
    {
        public static void Main(string[] args)
        {
            var message = Console.ReadLine();
            SendClientRequest(message);
        }

        private static void SendClientRequest(string message)
        {
            TcpClient client = new TcpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11418));
            client.Connect(IPAddress.Parse("127.0.0.1"), 11417);

            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            var stream = client.GetStream();

            // Send the message to the connected TcpServer. 
            stream.Write(data, 0, data.Length);

            // Close everything.
            stream.Close();
            client.Close();
        }
    }
}
