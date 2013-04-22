using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Serialization;

namespace IronStrife.MasterServer
{
    [Serializable]
    [XmlType("ServerInfo")]
    public class ServerInfo
    {
        [XmlElement("ipAddress")]
        public string ipAddress;
        public string IpAddress { get { return ipAddress; } }
        [XmlElement("port")]
        public int port;
        public string Port { get { return port.ToString(); } }
        [XmlElement("gameName")]
        public string gameName;
        public string GameName { get { return gameName; } }

        [XmlElement("gameType")]
        public string gametype;
        [XmlElement("gameDescription")]
        public string gameDescription;
        [XmlElement("numConnectedPlayers")]
        public int numConnectedPlayers;
        public string NumConnectedPlayers { get { return numConnectedPlayers.ToString(); } }
        [XmlElement("maxPlayers")]
        public int maxPlayers;

        public string SkillRating { get { return averageSkillRating.ToString(); } }
        [XmlIgnore]
        public int averageSkillRating = 100;

        public override string ToString()
        {
            return ipAddress + ": " + port + " | " + gameName + " : " + gameDescription + " | Type: " + gametype + " | " + numConnectedPlayers + " / " + maxPlayers;
        }

        internal void AddPlayer(int skillRating)
        {
            var oldTotalSkill = numConnectedPlayers * averageSkillRating;
            var newTotalSkill = oldTotalSkill + skillRating;
            numConnectedPlayers++;
            this.averageSkillRating = newTotalSkill / numConnectedPlayers;
        }

        internal void RemovePlayer(int skillRating)
        {
            var oldTotalSkill = numConnectedPlayers * averageSkillRating;
            var newTotalSkill = oldTotalSkill - skillRating;
            numConnectedPlayers--;
            if (numConnectedPlayers == 0)
                averageSkillRating = 100;
            else
                this.averageSkillRating = newTotalSkill / numConnectedPlayers;
        }
    }

    [Serializable]
    [XmlRoot("StrifeServerList")]
    public class StrifeServerList
    {
        public List<ServerInfo> servers;
    }
}