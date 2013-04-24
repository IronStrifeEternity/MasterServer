using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronStrife.Matchmaking;
using IronStrife.ChatServer;
using System.Diagnostics;

namespace MatchmakingTester
{
    class Program
    {

        static void Main(string[] args)
        {
            MockMatchmaker matchmaker = new MockMatchmaker();
            matchmaker.Add(new FakeSoloPlayer("Jonny", 100));
            matchmaker.Add(new FakeSoloPlayer("Jamie", 110));
            matchmaker.Add(new FakeSoloPlayer("Drake", 120));
            matchmaker.Add(new FakeSoloPlayer("Blaine", 90));
            matchmaker.Add(new FakeParty(2, 150));
            matchmaker.Add(new FakeSoloPlayer("Paul", 125));
            matchmaker.Add(new FakeParty(3, 120));
            matchmaker.Add(new FakeParty(4, 100));
            matchmaker.Add(new FakeParty(2, 90));
            matchmaker.Add(new FakeParty(2, 80));
            matchmaker.Add(new FakeSoloPlayer("Elayne", 150));
            matchmaker.Add(new FakeSoloPlayer("Delila", 140));
            matchmaker.Add(new FakeSoloPlayer("Samuel", 60));
            matchmaker.Add(new FakeSoloPlayer("Cynthia", 75));
            matchmaker.Add(new FakeSoloPlayer("Dale", 200));
            matchmaker.TryMakeMatch(150, 7);
            Console.Read();
        }
    }

    internal class MockMatchmaker
    {
        List<MatchmakingEntity> usersInQueue = new List<MatchmakingEntity>();
        public void Add(MatchmakingEntity entity) { usersInQueue.Add(entity); usersInQueue.Sort(); }

        public void TryMakeMatch(int skillThreshold, int teamSize)
        {
            Console.WriteLine("Initial Players (" + usersInQueue.Count + ") :");
            usersInQueue.ToList().ForEach(u => Console.WriteLine(u.ToString()));
            Console.WriteLine("---------------\n");
            Matchup matchup = this.FindMatchupFromIndex(0, teamSize, skillThreshold);
        }

        private Matchup FindMatchupFromIndex(int index, int teamSize, int skillThreshold)
        {
            if (index >= usersInQueue.Count) { return null; }
            var baseIndex = index;
            var matchup = new Matchup(teamSize);
            bool finished = false;
            while (!finished)
            {
                if (index >= usersInQueue.Count)
                {
                    matchup = null; break;
                }
                var entity = usersInQueue[index];
                matchup.TryAdd(entity);
                if (matchup.TeamsFull)
                {
                    finished = true;
                }
                index++;
            }
            if (matchup == null)
            {
                return FindMatchupFromIndex(baseIndex + 1, teamSize, skillThreshold);
            }
            else
            {
                Console.WriteLine("Matchup found: " + matchup.ToString());
                matchup.OptimizeTeams();
                if (matchup.IsMatchupBalanced(skillThreshold))
                {
                    return matchup;
                }
                else return FindMatchupFromIndex(baseIndex + 1, teamSize, skillThreshold);
            }
        }
    }

    public class FakeParty : MatchmakingEntity
    {
        public override void SendMessage(string message)
        {
            Console.WriteLine(message);
        }

        private int numUsers;
        public override int NumberOfUsers
        {
            get { return numUsers; }
        }

        private int skillRating;
        public override int SkillRating
        {
            get { return skillRating; }
        }
        public FakeParty(int numUsers, int skillRating) { this.numUsers = numUsers; this.skillRating = skillRating; }

        public override string ToString()
        {
            return "Party[" + NumberOfUsers + "] <" + SkillRating + ">";
        }
    }

    public class FakeSoloPlayer : MatchmakingEntity
    {
        public override void SendMessage(string message)
        {
            Console.WriteLine(message);
        }

        public override int NumberOfUsers
        {
            get { return 1; }
        }

        private string username;
        public override string ToString()
        {
            return username + " <" + SkillRating + ">";
        }
        private int skillRating;
        public override int SkillRating
        {
            get { return skillRating; }
        }
        public FakeSoloPlayer(string username, int skillRating) { this.skillRating = skillRating; this.username = username; }
    }




}
