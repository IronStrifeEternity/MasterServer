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

        internal static string InvitationMessage(string inviter, int inviterId)
        {
            return "invitation\n" + inviter + "\n" + inviterId;
        }

        internal static string UserJoinedPartyMessage(Connection connection)
        {
            return "partyjoined\n" + connection.username + "\n" + connection.userId;
        }

        internal static string PartyChangedMessage()
        {
            return "partychanged";
        }

        internal static string UserLeftRoomMessage(Connection connection)
        {
            return "userleft\n" + connection.username + "\n" + connection.userId;
        }

        internal static string MatchFoundMessage(MasterServer.ServerInfo server, int teamNumber)
        {
            return string.Format("matchfound\n{0}\n{1}\n{2}", server.IpAddress, server.port, teamNumber);
        }
    }
}
