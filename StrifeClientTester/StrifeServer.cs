using System.Net;
using System.Xml.Serialization;

public class StrifeServer
{
    [XmlElement("ipAddress")]
    public string ipAddress;
    [XmlElement("port")]
    public int port;
    [XmlElement("gameName")]
    public string gameName;
    [XmlElement("gameType")]
    public string gametype;
    [XmlElement("gameDescription")]
    public string gameDescription;
    [XmlElement("numConnectedPlayers")]
    public int numConnectedPlayers;
    [XmlElement("maxPlayers")]
    public int maxPlayers;
}