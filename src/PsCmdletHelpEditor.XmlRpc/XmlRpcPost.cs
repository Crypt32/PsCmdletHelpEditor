using System;
using CookComputing.XmlRpc;

namespace PsCmdletHelpEditor.XmlRpc {
    public class XmlRpcPost {
        [XmlRpcMember("postid")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public virtual String PostId { get; set; }
        [XmlRpcMember("title")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public virtual String Title { get; set; }
        [XmlRpcMember("description")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public virtual String HTML { get; set; }
        [XmlRpcMember("mt_text_more")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public virtual String TextMore { get; set; }
        [XmlRpcMember("mt_keywords")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public virtual String Keywords { get; set; }
        [XmlRpcMember("wp_slug")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public virtual String Slug { get; set; }
        [XmlRpcMember("mt_basename")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public virtual String BaseName { get; set; }
        [XmlRpcMember("mt_excerpt")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public virtual String Excerpt { get; set; }
        [XmlRpcMember("wp_author_id")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public virtual String AuthorID { get; set; }
        [XmlRpcMember("categories")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public virtual String[] Categories { get; set; }
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public virtual Boolean IsPublished { get; set; }
        [XmlRpcMember("dateCreated")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public virtual DateTime? DateCreated { get; set; }
        [XmlRpcMember("link")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public virtual String Permalink { get; set; }

        protected Boolean Equals(XmlRpcPost other) {
            return String.Equals(Title, other.Title);
        }

        public override Int32 GetHashCode() {
            return Title.GetHashCode();
        }
        public override Boolean Equals(Object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((XmlRpcPost)obj);
        }
    }
}