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
using System.Xml;
using System.Reflection;

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

        /// <summary>
        /// Gets all of the subclasses of a given type, using reflection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Type[] GetSubclasses<T>()
        {
            var allClasses = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(T))).ToArray();
            return allClasses;
        }

        private void HandleClientConnection(TcpClient client)
        {
            using (var stream = client.GetStream())
            {
                try
                {
                    var bytes = new byte[4096];
                    stream.Read(bytes, 0, bytes.Length);
                    var memStream = new MemoryStream(bytes);
                    var textReader = new XmlTextReader(memStream);
                    var data = Encoding.UTF8.GetString(bytes);
                    Type requestType = typeof(StrifeServerRequest);
                    foreach (Type t in GetSubclasses<StrifeServerRequest>())
                    {
                        if (data.Contains(t.Name))
                        {
                            Debug.Print("Match found: " + t.Name);
                            requestType = t;
                            break;
                        }
                    }

                    var request = new XmlSerializer(requestType).Deserialize(textReader) as StrifeServerRequest;
                    InterpretClientRequest(request, client);

                }
                catch (Exception e)
                {
                    this.PrintMessage("Exception: " + e.Message);
                }
            }
        }

        private void InterpretClientRequest(StrifeServerRequest request, TcpClient client)
        {
            PrintMessage("Interpreting client request: " + request.type);

            string requestType = request.type;
            switch (requestType)
            {
                case "GetServerList":
                    SendServerList(client);

                    break;
                case "RegisterServer":
                    var newServer = ((RegisterServerRequest)request).serverInfo;
                    newServer.ipAddress = client.Client.RemoteEndPoint.ToString().Split(':')[0];
                    RegisterNewServer(newServer);
                    break;
                case "DeregisterServer":
                    int port = ((DeregisterServerRequest)request).port;
                    TryDeregisterServer(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), port);
                    break;
                case "SendStats":
                    HandleStatsRequest((SendStatsRequest)request);
                    break;
                default:
                    WriteStringToClient(client, "Invalid command.");
                    break;
            }
        }

        private void HandleStatsRequest(SendStatsRequest sendStatsRequest)
        {
            PrintMessage("Not supported yet: SendStatsRequest.");
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
            else
            {
                PrintMessage("Error: Couldn't find server to deregister: " + ipAddress + " : " + port);
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
