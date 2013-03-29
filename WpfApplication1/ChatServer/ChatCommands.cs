using System.Text;
using System.Linq;

namespace IronStrife.ChatServer
{

    public static class ChatCommands
    {
        static void Global(StrifeChatServer server, string[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in parameters)
            {
                sb.Append(s + " ");
            }
            var totalMessage = sb.ToString();

            foreach (Connection c in server.OnlineConnections.Values)
            {
                c.SendMessage(totalMessage);
            }
        }

        static void Kick(StrifeChatServer server, string[] parameters)
        {
            var userToKick = server.GetUser(parameters[0]);
            if (userToKick == null) return;
            StringBuilder sb = new StringBuilder();
            foreach (string s in parameters.Skip(1))
            {
                sb.Append(s + " ");
            }
            var totalMessage = sb.ToString();
            userToKick.ForceDisconnect(totalMessage);
        }

        static void Whisper(StrifeChatServer server, string[] parameters)
        {
            var user = server.GetUser(parameters[0]);
            StringBuilder sb = new StringBuilder();
            foreach (string s in parameters.Skip(1))
            {
                sb.Append(s + " ");
            }
            var totalMessage = sb.ToString();
            user.SendMessage(totalMessage);
        }
    }
}