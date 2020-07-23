using System;
using CookComputing.XmlRpc;

namespace PsCmdletHelpEditor.XmlRpc.WordPress {
    public sealed class WpGetPost : IWpPost {
        [XmlRpcMember("post_id")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PostId { get; set; }

        [XmlRpcMember("post_status")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PostStatus { get; set; }

        [XmlRpcMember("post_type")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PostType { get; set; }

        [XmlRpcMember("post_parent")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PostParent { get; set; }

        [XmlRpcMember("post_title")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String Title { get; set; }

        /// <summary>
        /// Encoded URL (slug)
        /// </summary>
        [XmlRpcMember("post_name")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PageName { get; set; }

        [XmlRpcMember("post_content")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String HTML { get; set; }

        [XmlRpcMember("dateCreated")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public DateTime? DateCreated { get; set; }

        [XmlRpcMember("link")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String Permalink { get; set; }
    }
}