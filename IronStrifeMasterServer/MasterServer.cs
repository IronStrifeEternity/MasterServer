using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System;
using System.Diagnostics;

namespace IronStrifeMasterServer
{
    /// <summary>
    /// Class handling communication with clients, storing server information, and launching new server instances
    /// </summary>
    public class MasterServer
    {
        TcpListener listener;
        List<StrifeServer> servers;
        public bool isRunning;

        public MasterServer()
        {
            listener = new TcpListener(IPAddress.Any, Globals.listenPort);
            servers = new List<StrifeServer>();
        }

        /// <summary>
        /// Initializes and begins listening for client connections.
        /// </summary>
        public async void Start()
        {
            listener.Start();
            isRunning = true;
            while (true)
            {
                Console.WriteLine("LISTNEING!");
                var client = await listener.AcceptTcpClientAsync();
                HandleClientConnection(client);
            }
            isRunning = false;
        }

        private void HandleClientConnection(TcpClient client)
        {
            var stream = client.GetStream();
            var bytes = new byte[256];

            int i;
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                var data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                InterpretClientRequest(data);
            }

            client.Close();   // Shutdown and end connection
        }

        private void InterpretClientRequest(string data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a server item to the list
        /// </summary>
        /// <param name="server"></param>
        public void RegisterNewServer(StrifeServer server)
        {
            servers.Add(server);
        }

        /// <summary>
        /// Removes a server item from the list
        /// </summary>
        /// <param name="server"></param>
        public void UnregisterServer(StrifeServer server)
        {
            servers.Remove(server);
        }

        /// <summary>
        /// Launches a new server instance on the specified port
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private StrifeServer CreateNewServer(int port, string gameName, string description)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"C:\Users\Eric\Desktop\IronStrifeBuild\ISE.exe";
            startInfo.Arguments = String.Format("-batchmode StartHeadlessServer {0} {1} {2}", port, gameName, description);
            var process = Process.Start(startInfo);

            var server = new StrifeServer()
            {
                endPoint = new IPEndPoint(Globals.LocalAddress, port),
                gameDescription = description,
                gametype = "Default Iron Strife Game Type",
                gameName = gameName,
                maxPlayers = 20,
                numConnectedPlayers = 0               
            };

            return server;


        }
    }
}
