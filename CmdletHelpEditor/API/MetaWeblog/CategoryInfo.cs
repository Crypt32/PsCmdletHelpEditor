using System;

namespace CmdletHelpEditor.API.MetaWeblog {
	public class CategoryInfo {
		public Int32 categoryid { get; set; }
		public String title { get; set; }
		public String description { get; set; }
		public String htmlUrl { get; set; }
		public String rssUrl { get; set; }
	}
}