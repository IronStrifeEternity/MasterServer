using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronStrife.Matchmaking
{
    public class Matchup
    {
        public List<MatchmakingEntity> teamOne = new List<MatchmakingEntity>();
        public List<MatchmakingEntity> teamTwo = new List<MatchmakingEntity>();

        private bool IsTeamOneSmaller { get { return teamOne.Sum(e => e.NumberOfUsers) < teamTwo.Sum(e=>e.NumberOfUsers); } }

        private int teamSize;

        public Matchup(int numPlayers)
        {
            this.teamSize = numPlayers;
        }

        public void AddToTeamOne(MatchmakingEntity entity)
        {
            if (!teamOne.Contains(entity))
                teamOne.Add(entity);
        }

        public void AddToTeamTwo(MatchmakingEntity entity)
        {
            if (!teamTwo.Contains(entity))
                teamTwo.Add(entity);
        }

        public bool IsMatchupBalanced(int skillThreshold)
        {
            return AreTeamSkillsWithinThreshold(skillThreshold);
        }

        private bool AreTeamSkillsWithinThreshold(int skillThreshold)
        {

            var teamOneSkill = teamOne.Sum(t => t.TotalSkillRating);
            var teamTwoSkill = teamTwo.Sum(t => t.TotalSkillRating);
            Console.WriteLine("Team one has " + teamOneSkill + " skill rating and Team two has " + teamTwoSkill);
            if ((teamOneSkill - teamTwoSkill) <= skillThreshold)
            {
                return true;
            }

            return false;
        }

        public bool TryAdd(MatchmakingEntity entity)
        {
            if (IsTeamOneSmaller)
            {
                if (teamOne.Count + entity.NumberOfUsers <= this.teamSize)
                {
                    teamOne.Add(entity);
                    return true;
                }
            }
            else
            {
                if (teamTwo.Count + entity.NumberOfUsers <= this.teamSize)
                {
                    teamTwo.Add(entity);
                    return true;
                }
            }
            return false;
        }

        public bool TeamsFull { get { return (teamOne.Sum(e => e.NumberOfUsers) == teamSize && teamTwo.Sum(e => e.NumberOfUsers) == teamSize); } }

        public override string ToString()
        {
            var s = new StringBuilder("Matchup:\n---------------\n\n");
            s.Append("Team One: "); s.AppendLine(teamOne.Sum(e => e.TotalSkillRating).ToString());
            teamOne.ForEach(e => s.AppendLine(e.ToString()));
            s.AppendLine();
            s.Append("Team Two: "); s.AppendLine(teamTwo.Sum(e => e.TotalSkillRating).ToString());
            teamTwo.ForEach(e => s.AppendLine(e.ToString()));
            return s.ToString();
        }

        public void OptimizeTeams()
        {
            
        }
    }
}
