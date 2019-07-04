using System;
using CookComputing.XmlRpc;

namespace PsCmdletHelpEditor.XmlRpc {
    public class CategoryInfo {
        [XmlRpcMember("categoryid")]
        public Int32 CategoryID { get; set; }
        [XmlRpcMember("title")]
        public String Title { get; set; }
        [XmlRpcMember("description")]
        public String Description { get; set; }
        [XmlRpcMember("htmlUrl")]
        public String HtmlUrl { get; set; }

        [XmlRpcMember("rssUrl")]
        public String RssUrl { get; set; } = String.Empty;
    }
}