using CmdletHelpEditor.API.BaseClasses;
using CmdletHelpEditor.API.MetaWeblog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CmdletHelpEditor.API.Tools {
	static class MetaWeblogWrapper {
		public static Task<List<Post>> GetRecentPosts(Blogger blogger, IEnumerable<CmdletObject> cmdlets, Int32 fetchPostCount) {
			return Task<List<Post>>.Factory.StartNew(() => blogger.GetRecentPosts(fetchPostCount));
		}
		public async static Task PublishSingle(CmdletObject cmdlet, ModuleObject module, Blogger blogger, Boolean quiet) {
			Post post = new Post {
				title = cmdlet.Name,
				postid = cmdlet.ArticleIDString,
				description = await HtmlProcessor.GenerateHtmlView(cmdlet, module)
			};
			if (blogger == null) {
				blogger = Utils.InitializeBlogger(module.Provider);
			}
			if (blogger == null) {
				Utils.MsgBox("Warning", Strings.WarnBloggerNeedsMoreData, MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			if (String.IsNullOrEmpty(cmdlet.ArticleIDString)) {
				// assuming that article does not exist
				cmdlet.ArticleIDString = blogger.AddPost(post);
				if (!String.IsNullOrEmpty(cmdlet.ArticleIDString) && !quiet) {
					Utils.MsgBox("Success", (new Win32Exception(0)).Message, MessageBoxButton.OK, MessageBoxImage.Information);
				}
				// get post URL once published
				if (!String.IsNullOrEmpty(cmdlet.ArticleIDString)) {
					try {
						cmdlet.URL = module.Provider.ProviderName.ToLower() == "codeplex"
							? module.Provider.Blog.url + "wikipage?title=" + cmdlet.Name
							: blogger.GetPost(cmdlet.ArticleIDString).link;
					} catch { }
				}
			} else {
				// assuming that article exist, so we just change it
				Int32 result = blogger.UpdatePost(post);
				// 0x80131600 connect succeeds, but the post is deleted. Remove postid
				if (result == -2146232832) {
					cmdlet.ArticleIDString = null;
				}
			}
		}
		public async static void PublishAll(ModuleObject module, ProgressBar pb) {
			Blogger blogger = Utils.InitializeBlogger(module.Provider);
			if (blogger == null) {
				Utils.MsgBox("Warning", Strings.WarnBloggerNeedsMoreData, MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			List<CmdletObject> cmdletsToProcess = module.Cmdlets.Where(x => x.Publish).ToList();
			Double duration = 100.0 / cmdletsToProcess.Count;
			pb.Value = 0;
			foreach (CmdletObject cmdlet in cmdletsToProcess) {
				await PublishSingle(cmdlet, module, blogger, true);
				pb.Value += duration;
			}
			Utils.MsgBox("Success", (new Win32Exception(0)).Message, MessageBoxButton.OK, MessageBoxImage.Information);
			pb.Value = 100;
		}
	}
}
