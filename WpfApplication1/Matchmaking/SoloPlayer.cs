namespace IronStrife.Matchmaking
{
    using IronStrife.ChatServer;
    using System.Collections.Generic;

    public class SoloPlayer : MatchmakingEntity
    {
        internal Connection connection;

        public SoloPlayer(Connection connection)
        {
            this.connection = connection;
        }

        public override void SendMessage(string message)
        {
            connection.SendMessage(message);
        }

        public override int NumberOfUsers
        {
            get { return 1; }
        }
    }
}