﻿namespace IronStrife.Matchmaking
{
    public abstract class MatchmakingEntity
    {
        public abstract void SendMessage(string message);

        public abstract int NumberOfUsers { get; }
    } 
}