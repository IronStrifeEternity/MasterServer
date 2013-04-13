using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronStrife.ChatServer
{
    public static class ChatMessage
    {
        public static string RoomChangedMessage(ChatRoom room)
        {
            var message = "roomchanged\n" + room.Name;
            return message;
        }

        public static string UserJoinedRoomMessage(Connection conn)
        {
            return "userjoined\n" + conn.username + "\n" + conn.userId;
        }

        internal static string ChatFormat(string username, int userid, string message)
        {
            return "chatmessage\n" + username + "\n" + userid + "\n" + message;
        }
    }
}
