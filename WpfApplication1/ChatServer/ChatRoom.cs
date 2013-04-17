using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronStrife.ChatServer
{
    public class ChatRoom
    {
        public string Name { get; set; }

        private List<Connection> clientsInRoom = new List<Connection>();
        public List<Connection> ClientsInRoom
        {
            get
            {
                return clientsInRoom;
            }
        }

        public ChatRoom(string name)
        {
            this.Name = name;
        }

        internal void AddClient(Connection newConnection)
        {
            newConnection.SendMessage(ChatMessage.RoomChangedMessage(this));
            foreach (Connection conn in this.clientsInRoom)
            {
                conn.SendMessage(ChatMessage.UserJoinedRoomMessage(newConnection));
                newConnection.SendMessage(ChatMessage.UserJoinedRoomMessage(conn));
            }
            newConnection.SendMessage(ChatMessage.UserJoinedRoomMessage(newConnection));

            this.clientsInRoom.Add(newConnection);
            newConnection.CurrentRoom = this;
        }

        internal void RemoveClient(Connection connection)
        {
            this.clientsInRoom.Remove(connection);
            foreach (Connection conn in this.clientsInRoom)
            {
                conn.SendMessage(ChatMessage.UserLeftRoomMessage(connection));
            }
        }
    }

    public class ChatRoomCollection : List<ChatRoom>
    {
        public ChatRoom this[string name]
        {
            get
            {
                return this.Single((room) => room.Name == name);
            }
        }
    }
}
