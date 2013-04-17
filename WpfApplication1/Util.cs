using IronStrife.ChatServer;
using System.Collections.Generic;
namespace IronStrife
{
    public static class Util
    {
        public static void SendMessage(this List<Connection> connections, string message)
        {
            foreach (Connection connection in connections)
            {
                connection.SendMessage(message);
            }
        }
    }
}