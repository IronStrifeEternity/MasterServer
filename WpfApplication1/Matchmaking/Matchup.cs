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
            if (teamOne.Sum(t => t.NumberOfUsers) == teamTwo.Sum(t => t.NumberOfUsers))
            {
                var teamOneSkill = teamOne.Sum(t => t.SkillRating);
                var teamTwoSkill = teamTwo.Sum(t => t.SkillRating);
                Console.WriteLine("Team one has " + teamOneSkill + " skill rating and Team two has " + teamTwoSkill);
                if ((teamOneSkill - teamTwoSkill) <= skillThreshold)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
