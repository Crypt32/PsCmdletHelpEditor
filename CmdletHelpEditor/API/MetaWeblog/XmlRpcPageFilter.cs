using System;
using CookComputing.XmlRpc;

namespace CmdletHelpEditor.API.MetaWeblog {
    public class XmlRpcPageFilter {
        [XmlRpcMember("post_type")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PostType { get; set; }
        [XmlRpcMember("post_status")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PostStatus { get; set; }
        [XmlRpcMember("number")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public Int32 Number { get; set; }
        [XmlRpcMember("offset")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public Int32 Offset { get; set; }
        [XmlRpcMember("orderby")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String OrderBy { get; set; }
        [XmlRpcMember("order")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String Order { get; set; }
    }
}
