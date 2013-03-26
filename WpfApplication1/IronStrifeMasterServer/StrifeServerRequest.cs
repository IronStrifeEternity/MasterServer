using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IronStrife.MasterServer
{
    public class StrifeServerRequest
    {
        [XmlAttribute("type")]
        public RequestType type;
        public string[] parameters;
    }

    public enum RequestType
    {
        GetServerList,
        RegisterServer,
        DeregisterServer
    }
}
