using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alchemy;
using Alchemy.Classes;
using System.Collections.Concurrent;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace IronStrife.ChatServer
{
    public class StrifeChatServer
    {
        //Thread-safe collection of Online Connections.
        public ConcurrentDictionary<string, Connection> OnlineConnections = new ConcurrentDictionary<string, Connection>();

        public MTObservableCollection<string> ConnectedUsers = new MTObservableCollection<string>();
        public MTObservableCollection<string> ConsoleLogs = new MTObservableCollection<string>();

        private WebSocketServer aServer;

        public void Start()
        {
            // instantiate a new server - acceptable port and IP range,
            // and set up your methods.

            aServer = new WebSocketServer(8100, System.Net.IPAddress.Any)
            {
                OnReceive = OnReceive,
                OnSend = OnSend,
                OnConnected = OnConnect,
                OnDisconnect = OnDisconnect,
                TimeOut = new TimeSpan(0, 5, 0)
            };
            aServer.Start();

        }

        void SubmitCommand(string command)
        {
            if (command == "exit")
                aServer.Stop();
        }

        public void OnConnect(UserContext aContext)
        {
            AddToConsoleLog("Client Connected From : " + aContext.ClientAddress.ToString());

            // Create a new Connection Object to save client context information
            var conn = new Connection { Context = aContext };

            // Add a connection Object to thread-safe collection
            this.OnlineConnections.TryAdd(aContext.ClientAddress.ToString(), conn);
            this.ConnectedUsers.Add(aContext.ClientAddress.ToString());
        }

        void AddToConsoleLog(string message)
        {

            ConsoleLogs.Add(message);
        }


        public void OnReceive(UserContext aContext)
        {
            try
            {
                AddToConsoleLog("Data Received From [" + aContext.ClientAddress.ToString() + "] - " + aContext.DataFrame.ToString());
                BroadcastChatMessage(aContext.DataFrame.ToString());
            }
            catch (Exception ex)
            {
                AddToConsoleLog(ex.Message.ToString());
            }

        }

        private void BroadcastChatMessage(string message)
        {
            foreach (Connection c in OnlineConnections.Values)
            {
                c.SendMessage(message);
            }
        }

        public void OnSend(UserContext aContext)
        {
            AddToConsoleLog("Data Sent To : " + aContext.ClientAddress.ToString());
        }

        public void OnDisconnect(UserContext aContext)
        {
            AddToConsoleLog("Client Disconnected : " + aContext.ClientAddress.ToString());

            // Remove the connection Object from the thread-safe collection
            Connection conn;
            OnlineConnections.TryRemove(aContext.ClientAddress.ToString(), out conn);
            ConnectedUsers.Remove(aContext.ClientAddress.ToString());
        }




        internal void Stop()
        {
            aServer.Stop();
        }

        internal void SendGlobalChatMessage(string message)
        {
            foreach (Connection c in OnlineConnections.Values)
            {
                c.SendMessage(message);
            }
        }

        internal Connection GetUser(string ipAddress)
        {
            var user = OnlineConnections[ipAddress];
            return user;
        }

        internal void SubmitCommand(ChatCommand com)
        {
            Type t = typeof(ChatCommands);
            var method = t.GetMethod(com.name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
            if (method != null)
            {
                object[] parameters = new object[2];
                parameters[0] = this;
                parameters[1] = com.parameters;
                method.Invoke(null, parameters);
            }
        }
    }

    public class Connection
    {
        public UserContext Context { get; set; }
        public void SendMessage(string message)
        {
            Context.Send(message);
        }
        /// <summary>
        /// Forces a disconnect on this user for the given reason.
        /// </summary>
        public void ForceDisconnect(string message)
        {
            this.SendMessage("You were forcibly disconnected for the following reason: " + message);
            Context.OnDisconnect();
        }
    }
}