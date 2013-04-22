using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronStrife.ChatServer
{
    public class Party
    {
        List<Connection> usersInParty;
        public List<Connection> Users { get { return usersInParty; } }
        public ChatRoom room = new ChatRoom("Party Chat");

        public Party()
        {
            usersInParty = new List<Connection>();
        }

        public void AddToParty(Connection connection)
        {
            this.usersInParty.Add(connection);
            if (connection.party != null)
                connection.party.usersInParty.Remove(connection); // Remove user from old party, if he was in one
            connection.party = this;
            this.usersInParty.Add(connection);
            connection.SendMessage(ChatMessage.PartyChangedMessage());
            // Notify new member of all users in party.
            foreach (Connection partyMember in this.usersInParty)
            {
                connection.SendMessage(ChatMessage.UserJoinedPartyMessage(partyMember));
            }


        }
    }
}
