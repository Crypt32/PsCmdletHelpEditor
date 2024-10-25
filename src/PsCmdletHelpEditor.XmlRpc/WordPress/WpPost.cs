using System;
using CookComputing.XmlRpc;

namespace PsCmdletHelpEditor.XmlRpc.WordPress {
    public class WpPost : XmlRpcPost {
        [XmlRpcMember("post_type")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PostType { get; set; }
        [XmlRpcMember("post_parent")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PostParent { get; set; }
        [XmlRpcMember("post_content")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public override String HTML { get; set; }
    }
}