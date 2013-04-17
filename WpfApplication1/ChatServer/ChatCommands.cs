using System.Text;
using System.Linq;
using IronStrife.MasterServer;

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
            var userToKick = server.GetConnection(parameters[0]);
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
            var user = server.GetConnection(parameters[0]);
            StringBuilder sb = new StringBuilder();
            foreach (string s in parameters.Skip(1))
            {
                sb.Append(s + " ");
            }
            var totalMessage = sb.ToString();
            user.SendMessage(totalMessage);
        }

        static void Serialize(StrifeChatServer server, string[] parameters)
        {
            var bytes = Encoding.UTF8.GetBytes(@"<?xml version=""1.0"" encoding=""Windows-1252/""?><RegisterServerRequest xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" type=""RegisterServer"">  <serverInfo>    <port>25000</port>    <gameName>Enter game name here</gameName>    <gameType>DefaultGameType</gameType>    <gameDescription>Enter game description</gameDescription>    <numConnectedPlayers>0</numConnectedPlayers>    <maxPlayers>0</maxPlayers>  </serverInfo></RegisterServerRequest>");
            var memStream = new System.IO.MemoryStream(bytes);
            var obj = new System.Xml.Serialization.XmlSerializer(typeof(StrifeServerRequest)).Deserialize(memStream) as StrifeServerRequest;
            server.AddToConsoleLog(obj.type);
        }

        static void Launch(StrifeChatServer server, string[] parameters)
        {
            Launcher.LaunchServer();
        }
    }
}