using System;
using CookComputing.XmlRpc;

namespace CmdletHelpEditor.API.MetaWeblog {
    public class MediaObjectInfo {
        [XmlRpcMember("file")]
        public String File { get; set; }
        [XmlRpcMember("url")]
        public String Url { get; set; }
        [XmlRpcMember("type")]
        public String Type { get; set; }
    }
}