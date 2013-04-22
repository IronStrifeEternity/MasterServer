namespace IronStrife.Matchmaking
{
    public abstract class MatchmakingEntity
    {
        public abstract void SendMessage(string message);

        public abstract int NumberOfUsers { get; }

        public abstract int SkillRating { get; }

        public int skillThreshold = 10;
        public void IncrementSkillThreshold(int value) { skillThreshold += value; }
    }
}