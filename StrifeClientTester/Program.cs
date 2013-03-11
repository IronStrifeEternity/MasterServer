using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace IronStrifeMasterServer
{
    class ClientTester
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                var message = Console.ReadLine();
                if (message == "exit") break;
                SendClientRequest(message);
            }
        }

        private static void SendClientRequest(string message)
        {
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse("66.61.116.111"), 11417);
            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            var stream = client.GetStream();
            // Send the message to the connected TcpServer. 
            stream.Write(data, 0, data.Length);
            stream.Flush();

            var bytes = new byte[4096];
            stream.Read(bytes, 0, 4096);
            stream.Flush();
            XmlSerializer xs = new XmlSerializer(typeof(List<StrifeServer>));
            List<StrifeServer> servers = (List<StrifeServer>)xs.Deserialize(new MemoryStream(bytes));
            var response = System.Text.Encoding.ASCII.GetString(bytes);
            Console.WriteLine(response);
            Console.WriteLine("Server Names: ");
            foreach (StrifeServer s in servers)
            {
                Console.WriteLine(s.gameName + ": " + s.gameDescription);
            }

            // Close everything.
            client.Close();

        }
    }
}
