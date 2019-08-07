using System;
using CookComputing.XmlRpc;

namespace CmdletHelpEditor.API.MetaWeblog {
    public class WpPost {
        public virtual String PostType { get; set; }
        public virtual String Title { get; set; }
        public virtual String HTML { get; set; }
        //public virtual DateTime? DateCreated { get; set; }
    }
    public sealed class WpGetPost : WpPost {
        [XmlRpcMember("post_id")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PostId { get; set; }

        [XmlRpcMember("post_status")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PostStatus { get; set; }

        [XmlRpcMember("post_type")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public override String PostType { get; set; }

        [XmlRpcMember("post_parent")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PostParent { get; set; }

        [XmlRpcMember("post_title")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public override String Title { get; set; }

        /// <summary>
        /// Encoded URL (slug)
        /// </summary>
        [XmlRpcMember("post_name")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PageName { get; set; }

        [XmlRpcMember("post_content")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public override String HTML { get; set; }

        [XmlRpcMember("dateCreated")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public DateTime? DateCreated { get; set; }

        [XmlRpcMember("link")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String Permalink { get; set; }
    }

    public sealed class WpPostCreate : WpPost {
        [XmlRpcMember("post_status")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PostStatus { get; set; } = "publish";

        [XmlRpcMember("post_type")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public override String PostType { get; set; } = "page";

        [XmlRpcMember("post_parent")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public Int32 PostParent { get; set; }

        [XmlRpcMember("post_title")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public override String Title { get; set; }

        /// <summary>
        /// Encoded URL (slug)
        /// </summary>
        [XmlRpcMember("post_name")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PageName { get; set; }

        [XmlRpcMember("post_content")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public override String HTML { get; set; }
    }
    public sealed class WpPostUpdate : WpPost {
        [XmlRpcMember("post_status")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PostStatus { get; set; }

        [XmlRpcMember("post_type")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public override String PostType { get; set; }

        [XmlRpcMember("post_parent")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public Int32 PostParent { get; set; }

        [XmlRpcMember("post_title")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public override String Title { get; set; }
        
        /// <summary>
        /// Encoded URL (slug)
        /// </summary>
        [XmlRpcMember("post_name")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PageName { get; set; }

        [XmlRpcMember("post_content")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public override String HTML { get; set; }
    }
    public class Post<T> {
        [XmlRpcMember("postid")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public T PostId { get; set; }
        [XmlRpcMember("post_type")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PostType { get; set; }
        [XmlRpcMember("post_parent")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String PostParent { get; set; }
        [XmlRpcMember("title")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public String Title { get; set; }
        [XmlRpcMember("post_content")]
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