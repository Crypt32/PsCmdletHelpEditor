using System;
using CookComputing.XmlRpc;

namespace PsCmdletHelpEditor.XmlRpc {
    public class XmlRpcBlogInfo {
        [XmlRpcMember("blogid")]
        public String BlogID { get; set; }
        [XmlRpcMember("blogName")]
        public String BlogName { get; set; }
        [XmlRpcMember("url")]
        public String URL { get; set; }
    }
}
