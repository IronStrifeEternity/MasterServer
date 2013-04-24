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
        SortedSet<MatchmakingEntity> usersInQueue = new SortedSet<MatchmakingEntity>();
        public int TotalQueuedUsers { get { return usersInQueue.Sum(entity => entity.NumberOfUsers); } }
        StrifeMasterServer masterServer;
        Timer timer;

        public Matchmaker(StrifeMasterServer masterServer)
        {
            timer = new Timer(timerCallback, null, new TimeSpan(), new TimeSpan(0, 0, 5));
            this.masterServer = masterServer;
            if (masterServer == null)
            Debug.WriteLine("Master server is null." + masterServer);
        }

        private void timerCallback(object state)
        {
            Debug.WriteLine("Automatic matchmaking running now.");
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
                Debug.WriteLine(connection.username + " is in a party. His party is being added to the matchmaking queue.");
                AddPartyToQueue(connection.party);
            }
            else
            {
                Debug.WriteLine(connection.username + " is not in a party. Removing him from the solo queue.");
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
            if (connection == null) return;
            if (connection.party != null)
            {
                Debug.WriteLine(connection.username + " is in a party. His party is being removed from the queue.");
                RemovePartyFromQueue(connection.party);
            }
            else
            {
                SoloPlayer entity = GetSoloPlayer(usersInQueue, connection);
                if (entity != null)
                {
                    usersInQueue.Remove(entity);
                }
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

        public static SoloPlayer GetSoloPlayer(SortedSet<MatchmakingEntity> entities, Connection connection)
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
        public static MatchmakingParty GetParty(SortedSet<MatchmakingEntity> entities, Party party)
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
                LaunchNewServerWithMatchup(new List<MatchmakingEntity>(usersInQueue));
            var listCopy = new List<MatchmakingEntity>(usersInQueue);
            foreach (MatchmakingEntity entity in listCopy)
            {
                var server = TryFindExistingServer(entity, entity.skillThreshold);
                if (server != null)
                {
                    ConnectUsersToGame(entity, server, -1);
                }
            }
            foreach (MatchmakingEntity m in usersInQueue)
            {
                m.IncrementSkillThreshold(5);
            }
        }

        private void ConnectUsersToGame(MatchmakingEntity entity, ServerInfo server, int teamNumber)
        {
            entity.SendMessage(ChatMessage.MatchFoundMessage(server, teamNumber));
            RemoveEntityFromQueue(entity);
        }

        private ServerInfo TryFindExistingServer(MatchmakingEntity entity, int threshold)
        {
            Debug.WriteLine("Trying to find existing server for " + entity.ToString() + " which has skill rating " + entity.SkillRating + " with a threshold of " + threshold);
            foreach (ServerInfo server in masterServer.Servers)
            {
                Debug.WriteLine("SERVER: " + server.ToString());
                Debug.WriteLine("expression evaluates to " + (Math.Abs(server.averageSkillRating - entity.SkillRating) <= threshold));
                if (server.numConnectedPlayers + entity.NumberOfUsers <= server.maxPlayers)
                {

                    if (Math.Abs(server.averageSkillRating - entity.SkillRating) <= threshold)
                    {
                        return server;
                    }
                }
                else
                    Debug.WriteLine("server has too many people.");

            }
            return null;
        }

        private void LaunchNewServerWithMatchup(List<MatchmakingEntity> users)
        {
            var server = Launcher.LaunchServer();
            int team = 1;
            foreach (MatchmakingEntity entity in users)
            {
                entity.SendMessage(ChatMessage.MatchFoundMessage(server, (++team % 2) + 1));
                RemoveEntityFromQueue(entity);
            }
        }
    }
}
