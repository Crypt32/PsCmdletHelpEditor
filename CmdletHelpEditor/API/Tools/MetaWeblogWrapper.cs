using CmdletHelpEditor.API.MetaWeblog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CmdletHelpEditor.API.Models;

namespace CmdletHelpEditor.API.Tools {
	static class MetaWeblogWrapper {
		public static Task<List<Post>> GetRecentPosts(Blogger blogger, IEnumerable<CmdletObject> cmdlets, Int32 fetchPostCount) {
			return Task<List<Post>>.Factory.StartNew(() => blogger.GetRecentPosts(fetchPostCount));
		}
		public static Task PublishSingle(CmdletObject cmdlet, ModuleObject module, Blogger blogger, Boolean quiet) {
            return Task.Factory.StartNew(() => {
                Post post = new Post {
                    Title = cmdlet.Name,
                    PostId = cmdlet.ArticleIDString,
                    HTML = HtmlProcessor.GenerateHtmlView(cmdlet, module).Result
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
                        Utils.MsgBox("Success", new Win32Exception(0).Message, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    // get post URL once published
                    if (!String.IsNullOrEmpty(cmdlet.ArticleIDString)) {
                        try {
                            cmdlet.URL = module.Provider.ProviderName.ToLower() == "codeplex"
                                ? module.Provider.Blog.url + "wikipage?title=" + cmdlet.Name
                                : blogger.GetPost(cmdlet.ArticleIDString).Permalink;
                            if (!Uri.IsWellFormedUriString(cmdlet.URL, UriKind.Absolute)) {
                                var baseUrl = new Uri(module.Provider.ProviderURL);
                                cmdlet.URL = String.Format("{0}://{1}{2}", baseUrl.Scheme, baseUrl.DnsSafeHost, cmdlet.URL);
                            }
                        } catch { }
                    }
                } else {
                    try {
                        // assuming that article exist, so we just change it
                        blogger.UpdatePost(post);
                    } catch (Exception e) {
                        // 0x80131600 connect succeeds, but the post is deleted. Remove postid
                        if (e.HResult == -2146232832 || e.HResult == -2147023728) {
                            cmdlet.ArticleIDString = null;
                        }
                        throw;
                    }
                }
            });
		}
		public static async void PublishAll(ModuleObject module, ProgressBar pb) {
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
