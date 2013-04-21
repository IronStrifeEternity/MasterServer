using IronStrife.ChatServer;
using IronStrife.MasterServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace IronStrife.Matchmaking
{
    public class Matchmaker
    {
        private List<MatchmakingEntity> usersInQueue = new List<MatchmakingEntity>();
        public int TotalQueuedUsers { get { return usersInQueue.Sum(entity => entity.NumberOfUsers); } }

        public Matchmaker()
        {
            new Timer(timerCallback, null, new TimeSpan(), new TimeSpan(0, 0, 5));
        }

        private void timerCallback(object state)
        {
            TryFindMatch();
        }

        public void AddUserToQueue(Connection connection)
        {
            var oldEntity = GetSoloPlayer(usersInQueue, connection);
            if (oldEntity != null)
            {
                Debug.WriteLine(connection.username + " has attempted to join the matchmaking queue twice.");
                return;
            }

            if (connection.party != null)
            {
                AddPartyToQueue(connection.party);
            }
            else
            {
                var entity = new SoloPlayer(connection);
                usersInQueue.Add(entity);
            }

            TryFindMatch();
        }
        public void AddPartyToQueue(Party party)
        {
            var oldParty = GetParty(usersInQueue, party);
            if (oldParty != null)
            {
                Debug.WriteLine("The party with " + party.Users[0].username + " tried to join matchmaking twice.");
                return;
            }
            var newParty = new MatchmakingParty(party);
            usersInQueue.Add(newParty);
            TryFindMatch();
        }

        public void RemoveUserFromQueue(Connection connection)
        {
            if (connection.party != null)
            {
                RemovePartyFromQueue(connection.party);
            }

            SoloPlayer entity = GetSoloPlayer(usersInQueue, connection);
            if (entity != null)
            {
                usersInQueue.Remove(entity);
            }
        }
        public void RemovePartyFromQueue(Party party)
        {
            MatchmakingParty matchmakingParty = GetParty(usersInQueue, party);
            if (matchmakingParty != null)
            {
                usersInQueue.Remove(matchmakingParty);
            }
        }

        public static SoloPlayer GetSoloPlayer(List<MatchmakingEntity> entities, Connection connection)
        {
            foreach (SoloPlayer soloPlayer in entities)
            {
                if (soloPlayer.connection == connection)
                {
                    return soloPlayer;
                }
            }
            return null;
        }
        public static MatchmakingParty GetParty(List<MatchmakingEntity> entities, Party party)
        {
            foreach (MatchmakingParty matchmakingParty in entities)
            {
                if (matchmakingParty.party == party)
                {
                    return matchmakingParty;
                }
            }
            return null;
        }


        private void RemoveEntityFromQueue(MatchmakingEntity entity)
        {
            if (usersInQueue.Contains(entity))
            {
                usersInQueue.Remove(entity);
            }
        }

        private void TryFindMatch()
        {
            Console.WriteLine("Trying to find a match.");
            if (TotalQueuedUsers >= 2)
                MakeMatch(new List<MatchmakingEntity>(usersInQueue));

            usersInQueue.ForEach((m) => m.IncrementSkillThreshold(5));
        }

        private void MakeMatch(List<MatchmakingEntity> users)
        {
            var server = Launcher.LaunchServer();
            foreach (MatchmakingEntity entity in users)
            {
                entity.SendMessage(ChatMessage.MatchFoundMessage(server, 1));
                RemoveEntityFromQueue(entity);
            }
        }
    }
}
