using System.Collections.Generic;

namespace IronStrife.MasterServer
{
    public class PlayerStatRecord
    {
        public int playerId;
        public int numKills;
        public int numDeaths;
        //public Dictionary<string, int> items;
        //public Dictionary<string, int> spells;

        public PlayerStatRecord(int playerId, int numKills, int numDeaths, Dictionary<string, int> items, Dictionary<string, int> spells)
        {
            this.playerId = playerId;
            this.numKills = numKills;
            this.numDeaths = numDeaths;
            //this.items = items;
            //this.spells = spells;
        }

        public PlayerStatRecord()
        {
            this.playerId = -1;
            this.numKills = 0;
            this.numDeaths = 0;
            //this.items = new Dictionary<string, int>();
            //this.spells = new Dictionary<string, int>();
        }
    }
}