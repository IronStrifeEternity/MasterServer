using System.Collections.Generic;
using System.Net;
using System.Xml.Serialization;

[XmlType("ServerInfo")]
public class ServerInfo
{
    [XmlElement("ipAddress")]
    public string ipAddress;
    public string IpAddress { get { return ipAddress; } }
    [XmlElement("port")]
    public int port;
    [XmlElement("gameName")]
    public string gameName;
    public string GameName { get { return gameName; } }

    [XmlElement("gameType")]
    public string gametype;
    [XmlElement("gameDescription")]
    public string gameDescription;
    [XmlElement("numConnectedPlayers")]
    public int numConnectedPlayers;
    [XmlElement("maxPlayers")]
    public int maxPlayers;

    public override string ToString()
    {
        return ipAddress + ": " + port + " | " + gameName + " : " + gameDescription + " | Type: " + gametype + " | " + numConnectedPlayers + " / " + maxPlayers;
    }
}

[XmlRoot("StrifeServerList")]
public class StrifeServerList
{
    public List<ServerInfo> servers;
}