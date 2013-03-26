using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;

namespace IronStrife.MasterServer
{
    /// <summary>
    /// Class handling communication with clients, storing server information, and launching new server instances
    /// </summary>
    public class StrifeMasterServer
    {
        TcpListener listener;
        ObservableCollection<ServerInfo> servers;
        public ObservableCollection<ServerInfo> Servers { get { return servers; } }
        public bool isRunning;
        public delegate void OnMessageEventHandler(string message);
        public event OnMessageEventHandler OnMessage;

        public ObservableCollection<ServerInfo> ServerList
        {
            get
            {
                return new ObservableCollection<ServerInfo>(servers);
            }
        }

        public StrifeMasterServer()
        {
            listener = new TcpListener(IPAddress.Any, Globals.listenPort);
            servers = new ObservableCollection<ServerInfo>();

        }

        public void Stop()
        {
            isRunning = false;
        }

        /// <summary>
        /// Initializes and begins listening for client connections.
        /// </summary>
        public async void Start()
        {
            PrintMessage("Starting Master Server.");
            listener.Start();
            isRunning = true;
            while (isRunning)
            {
                var client = await listener.AcceptTcpClientAsync();
                HandleClientConnection(client);
            }
            listener.Stop();
        }

        void PrintMessage(string message)
        {
            if (OnMessage != null)
            {
                OnMessage(message);
            }
        }

        private void HandleClientConnection(TcpClient client)
        {
            var stream = client.GetStream();
            var bytes = new byte[256];

            int i;
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                var data = Encoding.ASCII.GetString(bytes, 0, i);
                InterpretClientRequest(client, data);
            }

            stream.Close();
            client.Close();   // Shutdown and end connection
        }

        private void InterpretClientRequest(TcpClient client, string data)
        {
            PrintMessage("Interpreting client request: " + data);
            var words = data.Split(' ');
            RequestType requestType;
            if (Enum.TryParse<RequestType>(words[0], true, out requestType))
            {
                switch (requestType)
                {
                    case RequestType.GetServerList:
                        SendServerList(client);

                        break;
                    case RequestType.RegisterServer:
                        var newServer = new ServerInfo()
                        {
                            port = int.Parse(words[1]),
                            gameName = words[2],
                            gameDescription = words[3],
                            gametype = words[4],
                            ipAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(),
                            maxPlayers = 20,
                            numConnectedPlayers = 0,

                        };
                        RegisterNewServer(newServer);
                        break;
                    case RequestType.DeregisterServer:
                        int port;
                        if (int.TryParse(words[1], out port))
                        {
                            TryDeregisterServer(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), port);
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid command: " + data);
                        break;
                }
            }
            else
            {
                WriteStringToClient(client, "Invalid command.");
            }
        }

        private void TryDeregisterServer(string ipAddress, int port)
        {
            PrintMessage("Trying to deregister server at " + ipAddress + ":" + port);
            ServerInfo serverToDeregister = null;
            foreach (ServerInfo si in servers)
            {
                if (si.port == port && si.ipAddress == ipAddress)
                {
                    serverToDeregister = si;
                    break;
                }
            }
            if (serverToDeregister != null)
            {
                servers.Remove(serverToDeregister);
            }
        }

        private void SendServerList(TcpClient client)
        {
            XmlSerializer xs = new XmlSerializer(servers.GetType());
            var stream = client.GetStream();
            xs.Serialize(stream, servers);
            stream.Flush();
        }

        /// <summary>
        /// Adds a server item to the list
        /// </summary>
        /// <param name="server"></param>
        public void RegisterNewServer(ServerInfo server)
        {
            PrintMessage("Registered new server: " + server.ToString());
            servers.Add(server);
        }

        /// <summary>
        /// Removes a server item from the list
        /// </summary>
        /// <param name="server"></param>
        public void UnregisterServer(ServerInfo server)
        {
            servers.Remove(server);
        }

        /// <summary>
        /// Launches a new server instance on the specified port
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private ServerInfo CreateNewServer(int port, string gameName, string description)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"C:\Users\Eric\Desktop\IronStrifeBuild\ISE.exe";
            startInfo.Arguments = String.Format("-batchmode StartHeadlessServer {0} {1} {2}", port, gameName, description);
            var process = Process.Start(startInfo);

            var server = new ServerInfo()
            {
                ipAddress = Globals.LocalAddress.ToString(),
                port = port,
                gameDescription = description,
                gametype = "Default Iron Strife Game Type",
                gameName = gameName,
                maxPlayers = 20,
                numConnectedPlayers = 0
            };

            return server;


        }

        private static void WriteStringToClient(TcpClient client, string message)
        {
            var stream = client.GetStream();
            var bytes = Encoding.UTF8.GetBytes(message);
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
        }

        static void Main(string[] args)
        {
            var server = new StrifeMasterServer();
            server.Start();
        }
    }
}
