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
            return AreTeamSkillsWithinThreshold(skillThreshold) && ArePairsBalanced(50);
        }

        private bool ArePairsBalanced(int threshold)
        {
            var combinations = from item in teamOne
                               from item2 in teamTwo
                               where item != item2
                               select new[] { item, item2 };

            foreach (MatchmakingEntity[] pair in combinations)
            {
                if (Math.Abs(pair[0].SkillRating - pair[1].SkillRating) > threshold)
                    return false;
            }
            return true;
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
            var soloPlayerCombinations = from item in teamOne
                                         from item2 in teamTwo
                                         where (item != item2 && item.NumberOfUsers == 1 && item2.NumberOfUsers == 1)
                                         select new[] { item, item2 };

            var possibleSwitches = new List<MatchmakingEntity[]>();
            var disparity = Math.Abs(this.teamOne.Sum(x => x.TotalSkillRating) - this.teamTwo.Sum(x => x.TotalSkillRating));

            foreach (MatchmakingEntity[] pairs in soloPlayerCombinations)
            {
                if (Math.Abs(pairs[0].SkillRating - pairs[1].SkillRating) <= disparity)
                {
                    possibleSwitches.Add(pairs);
                }
            }

            foreach (MatchmakingEntity[] pair in possibleSwitches)
            {
                var previousDisparity = Math.Abs(this.teamOne.Sum(x => x.TotalSkillRating) - this.teamTwo.Sum(x => x.TotalSkillRating));
                SwitchPair(pair);
                var afterDisparity = Math.Abs(this.teamOne.Sum(x => x.TotalSkillRating) - this.teamTwo.Sum(x => x.TotalSkillRating));
                if (disparity > previousDisparity)
                {
                    SwitchPair(pair);
                    break;
                }
            }
        }

        private void SwitchPair(MatchmakingEntity[] pair)
        {
            if (teamOne.Contains(pair[0]))
            {
                teamOne.Remove(pair[0]);
                teamOne.Add(pair[1]);
                teamTwo.Remove(pair[1]);
                teamTwo.Add(pair[0]);
            }
            else
            {
                teamOne.Remove(pair[1]);
                teamOne.Add(pair[0]);
                teamTwo.Remove(pair[0]);
                teamTwo.Add(pair[1]);

            }
        }
    }
}
