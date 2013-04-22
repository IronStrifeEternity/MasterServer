namespace IronStrife.Matchmaking
{
    using IronStrife.ChatServer;
    using System.Collections.Generic;
    using System.Linq;
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

        public override int SkillRating
        {
            get { return party.Users.Sum((u) => u.skillRating) / party.Users.Count; }
        }
    }
}