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
        private int maxConnections = defaultMaxConnections;
        const int defaultMaxConnections = 100;
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

        internal void AddClient(Connection conn)
        {
            this.clientsInRoom.Add(conn);
        }

        internal void RemoveClient(Connection connection)
        {
            this.clientsInRoom.Remove(connection);
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
