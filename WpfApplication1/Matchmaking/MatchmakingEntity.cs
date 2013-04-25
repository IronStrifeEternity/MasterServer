using System;
namespace IronStrife.Matchmaking
{
    public abstract class MatchmakingEntity : IComparable<MatchmakingEntity>
    {
        public abstract void SendMessage(string message);

        public abstract int NumberOfUsers { get; }

        public abstract int SkillRating { get; }

        public int skillThreshold = 10;
        public void IncrementSkillThreshold(int value) { skillThreshold += value; }

        public int CompareTo(MatchmakingEntity other)
        {
            if (other.SkillRating == this.SkillRating)
                return 0;
            if (other.SkillRating - this.SkillRating > 0)
            {
                return -1;
            }
            else
                return 1;
        }

        public abstract int TotalSkillRating { get;}
    }
}