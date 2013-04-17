using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IronStrife.MasterServer
{
    [XmlInclude(typeof(GetServerListRequest))]
    [XmlInclude(typeof(RegisterServerRequest))]
    [XmlInclude(typeof(DeregisterServerRequest))]
    [XmlInclude(typeof(SendStatsRequest))]
    public abstract class StrifeServerRequest
    {
        [XmlAttribute("type")]
        public string type;
    }

    [XmlRoot("GetServerListRequest")]
    public class GetServerListRequest : StrifeServerRequest
    {
        public GetServerListRequest()
        {
            type = "GetServerList";
        }
    }

    [XmlRoot("RegisterServerRequest")]
    public class RegisterServerRequest : StrifeServerRequest
    {
        public ServerInfo serverInfo;
        public RegisterServerRequest()
        {
            this.type = "RegisterServer";
        }
    }

    [XmlRoot("DeregisterServerRequest")]
    public class DeregisterServerRequest : StrifeServerRequest
    {
        public int port;
        public DeregisterServerRequest()
        {
            this.type = "DeregisterServer";
        }
    }

    [XmlRoot("SendStatsRequest")]
    public class SendStatsRequest : StrifeServerRequest
    {
        public PlayerStatRecord record;
        public SendStatsRequest()
        {
            this.type = "SendStats";
        }
    }
}
