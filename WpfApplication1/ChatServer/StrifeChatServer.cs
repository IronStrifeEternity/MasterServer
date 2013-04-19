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

        public ChatRoomCollection chatRooms;

        private WebSocketServer aServer;
        private Matchmaking.Matchmaker matchmaker;

        private List<Party> parties = new List<Party>();

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

            chatRooms = new ChatRoomCollection();
            chatRooms.Add(new ChatRoom("US Public Chat"));
            aServer.Start();

            matchmaker = new Matchmaking.Matchmaker();

        }

        void SubmitCommand(string command)
        {
            if (command == "exit")
                aServer.Stop();
        }

        public void OnConnect(UserContext aContext)
        {
            AddToConsoleLog("Client Connected From : " + aContext.ClientAddress.ToString());

            // Create a new Connection object to save client context information
            var conn = new Connection { Context = aContext };

            // Add a Connection object to thread-safe collection
            this.OnlineConnections.TryAdd(aContext.ClientAddress.ToString(), conn);
            this.ConnectedUsers.Add(aContext.ClientAddress.ToString());
        }

        internal void AddToConsoleLog(string message)
        {
            ConsoleLogs.Add(message);
        }


        public void OnReceive(UserContext aContext)
        {
            //try
            //{
                AddToConsoleLog("Data Received From [" + aContext.ClientAddress.ToString() + "] - " + aContext.DataFrame.ToString());
                HandleMessage(aContext.DataFrame.ToString(), aContext);
            //}
            //catch (Exception ex)
            //{
            //    AddToConsoleLog(ex.Message.ToString());
            //}
        }

        private void HandleMessage(string message, UserContext context)
        {
            var connection = GetConnection(context);
            var words = message.Split(' ');
            string[] parameters = new string[words.Length - 1];
            for (int g = 0; g < parameters.Length; g++)
            {
                parameters[g] = words[g + 1];
            }
            switch (words[0])
            {
                case "chat":
                    HandleChatMessage(connection, parameters, connection.CurrentRoom);
                    break;
                case "joinroom":
                    HandleJoinRoom(parameters, connection);
                    break;
                case "setuserid":
                    HandleSetUserIdMessage(parameters, connection);
                    break;
                case "setusername":
                    HandleSetUsername(parameters, connection);
                    break;
                case "invite":
                    HandleInvite(parameters, connection);
                    break;
                case "acceptinvite":
                    HandleAcceptInvite(parameters, connection);
                    break;
                case "joinmatchmaking":
                    HandleJoinMatchmaking(parameters, connection);
                    break;
                case "stopmatchmaking":
                    HandleStopMatchmaking(parameters, connection);
                    break;
                default:
                    AddToConsoleLog("Invalid command received from " + context.ClientAddress.ToString() + ": " + message);
                    break;
            }
        }

        private void HandleStopMatchmaking(string[] parameters, Connection connection)
        {
            matchmaker.RemoveUserFromQueue(connection);
        }

        private void HandleJoinMatchmaking(string[] parameters, Connection connection)
        {
            matchmaker.AddUserToQueue(connection);
        }

        private void HandleAcceptInvite(string[] parameters, Connection connection)
        {
            var inviter = GetConnectionByUsername(parameters[0]);
            var party = inviter.party;
            if (party == null)
            {
                party = new Party();
                parties.Add(party);
                MoveUserToRoom(inviter, party.room);
                party.AddToParty(inviter);                
            }

            // Add user to party and move him into the chat room.
            MoveUserToRoom(connection, party.room);
            party.AddToParty(connection);            

        }

        private void HandleInvite(string[] parameters, Connection connection)
        {
            var userToInvite = GetConnectionByUsername(parameters[0]);
            userToInvite.SendMessage(ChatMessage.InvitationMessage(connection.username, connection.userId));
        }

        private void HandleJoinRoom(string[] parameters, Connection connection)
        {
            var roomName = parameters[0];
            var room = GetRoomByName(roomName);
            if (room != null)
            {
                MoveUserToRoom(connection, room);
            }
        }

        /// <summary>
        /// Moves a user to the given ChatRoom
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="room"></param>
        private static void MoveUserToRoom(Connection connection, ChatRoom room)
        {
            if (connection.CurrentRoom != null)
            {
                connection.CurrentRoom.RemoveClient(connection);
            }
            room.AddClient(connection);


        }

        private void HandleSetUsername(string[] parameters, Connection connection)
        {
            var newUsername = parameters[0];
            if (newUsername != null && newUsername.Length > 0)
            {
                connection.username = newUsername;
                MoveUserToRoom(connection, chatRooms["US Public Chat"]);
            }
        }

        private void HandleSetUserIdMessage(string[] parameters, Connection connection)
        {
            int userId = -1;
            if (int.TryParse(parameters[0], out userId))
            {
                connection.userId = userId;
            }
            else
            {
                AddToConsoleLog("Error setting userId: Given Id is not formatted properly.");
            }
        }

        private ChatRoom GetRoom(UserContext context)
        {
            var conn = GetConnection(context);
            foreach (ChatRoom room in chatRooms)
            {
                if (room.ClientsInRoom.Contains(conn))
                {
                    return room;
                }
            }
            return null;
        }

        private ChatRoom GetRoomByName(string name)
        {
            foreach (ChatRoom room in chatRooms)
            {
                if (room.Name == name)
                {
                    return room;
                }
            }
            return null;
        }

        private void HandleChatMessage(Connection connection, string[] parameters, ChatRoom room)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in parameters)
            {
                sb.Append(s);
                sb.Append(' ');
            }
            BroadcastChatMessageToRoom(connection.username, connection.userId, sb.ToString(), room);
        }

        private void BroadcastChatMessageToRoom(string username, int userid, string message, ChatRoom room)
        {
            foreach (Connection c in room.ClientsInRoom)
            {
                c.SendMessage(ChatMessage.ChatFormat(username, userid, message));
            }
        }

        private void BroadcastGlobalChatMessage(string message)
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
            var room = conn.CurrentRoom;
            if (room != null)
            {
                room.RemoveClient(conn);
            }
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

        internal Connection GetConnection(string ipAddress)
        {
            var user = OnlineConnections[ipAddress];
            return user;
        }
        private Connection GetConnection(UserContext context)
        {
            foreach (Connection conn in OnlineConnections.Values)
            {
                if (conn.Context == context)
                {
                    return conn;
                }
            }
            return null;
        }
        internal Connection GetConnectionByUsername(string username)
        {
            var user = OnlineConnections.Single((c) => c.Value.username == username);
            return user.Value;
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
                try
                {
                    method.Invoke(null, parameters);
                }
                catch (Exception e)
                {
                    AddToConsoleLog(e.Message);

                }
            }
        }
    }

    public class Connection
    {
        /// <summary>
        /// The WebSocket UserContext associated with this connection.
        /// </summary>
        public UserContext Context { get; set; }
        /// <summary>
        /// The IronStrife user id associated with this connection.
        /// </summary>
        public int userId;
        /// <summary>
        /// The IronStrife username associated with this connection.
        /// </summary>
        public string username;
        /// <summary>
        /// The current matchmaking party this user is a member of.
        /// </summary>
        public Party party;

        public ChatRoom CurrentRoom { get; set; }

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