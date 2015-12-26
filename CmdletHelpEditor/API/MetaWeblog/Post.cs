using System;
using CookComputing.XmlRpc;

namespace CmdletHelpEditor.API.MetaWeblog {
	public class Post {
		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public String postid { get; set; }
		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public String title { get; set; }
		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public String description { get; set; }
		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public String mt_text_more { get; set; }
		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public String mt_keywords { get; set; }
		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public String wp_slug { get; set; }
		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public String mt_basename { get; set; }
		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public String mt_excerpt { get; set; }
		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public String wp_author_id { get; set; }
		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public String[] categories { get; set; }
		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public Boolean IsPublished { get; set; }
		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public DateTime dateCreated { get; set; }
		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public String link { get; set; }

		protected Boolean Equals(Post other) {
			return String.Equals(title, other.title);
		}

		public override Int32 GetHashCode() {
			return title.GetHashCode();
		}
		public override Boolean Equals(object obj) {
			if (ReferenceEquals(null, obj)) { return false; }
			if (ReferenceEquals(this, obj)) { return true; }
			return obj.GetType() == GetType() && Equals((Post) obj);
		}
	}
}