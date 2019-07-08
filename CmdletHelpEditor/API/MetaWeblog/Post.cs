using System;
using CookComputing.XmlRpc;

namespace CmdletHelpEditor.API.MetaWeblog {
    public class Post<T> {
        [XmlRpcMember("postid")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public T PostId { get; set; }
        [XmlRpcMember("title")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String Title { get; set; }
        [XmlRpcMember("description")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String HTML { get; set; }
        [XmlRpcMember("mt_text_more")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String TextMore { get; set; }
        [XmlRpcMember("mt_keywords")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String Keywords { get; set; }
        [XmlRpcMember("wp_slug")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String Slug { get; set; }
        [XmlRpcMember("mt_basename")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String BaseName { get; set; }
        [XmlRpcMember("mt_excerpt")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String Excerpt { get; set; }
        [XmlRpcMember("wp_author_id")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String AuthorID { get; set; }
        [XmlRpcMember("categories")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String[] Categories { get; set; }
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public Boolean IsPublished { get; set; }
        [XmlRpcMember("dateCreated")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public DateTime? DateCreated { get; set; }
        [XmlRpcMember("link")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String Permalink { get; set; }

        protected Boolean Equals(Post<T> other) {
            return String.Equals(Title, other.Title);
        }

        public override Int32 GetHashCode() {
            return Title.GetHashCode();
        }
        public override Boolean Equals(Object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Post<T>)obj);
        }
    }
}