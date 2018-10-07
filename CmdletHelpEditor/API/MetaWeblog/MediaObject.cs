using System;
using CookComputing.XmlRpc;

namespace CmdletHelpEditor.API.MetaWeblog {
    public class MediaObject {
        [XmlRpcMember("name")]
        public String Name { get; set; }
        [XmlRpcMember("type")]
        public String Type { get; set; }
        [XmlRpcMember("bits")]
        public Byte[] Bits { get; set; }
    }
}