namespace IronStrife.Matchmaking
{
    using IronStrife.ChatServer;
    using System.Collections.Generic;

    public class MatchmakingParty : MatchmakingEntity
    {
        internal Party party;

        public MatchmakingParty(Party party)
        {
            this.party = party;
        }

        public override void SendMessage(string message)
        {
            party.Users.SendMessage(message);
        }

        public override int NumberOfUsers
        {
            get { return party.Users.Count; }
        }
    }
}