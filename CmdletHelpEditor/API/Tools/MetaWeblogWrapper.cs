using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CmdletHelpEditor.API.MetaWeblog;
using CmdletHelpEditor.API.Models;

namespace CmdletHelpEditor.API.Tools {
    static class MetaWeblogWrapper {
        public static Task<List<Post<String>>> GetRecentPosts(Blogger blogger, IEnumerable<CmdletObject> cmdlets, Int32 fetchPostCount) {
            return Task<List<Post<String>>>.Factory.StartNew(() => blogger.GetRecentPosts(fetchPostCount));
        }
        public static Task PublishSingle(CmdletObject cmdlet, ModuleObject module, Blogger blogger, Boolean quiet) {
            return Task.Factory.StartNew(() => {
                Int32.TryParse(cmdlet.ArticleIDString, out Int32 id);
                var post = new Post<Int32> {
                    Title = cmdlet.Name,
                    PostId = id,
                    HTML = HtmlProcessor.GenerateHtmlView(cmdlet, module).Result
                };
                if (blogger == null) {
                    blogger = Utils.InitializeBlogger(module.Provider);
                }
                if (blogger == null) {
                    throw new Exception(Strings.WarnBloggerNeedsMoreData);
                }
                if (String.IsNullOrEmpty(cmdlet.ArticleIDString)) {
                    // assuming that article does not exist
                    cmdlet.ArticleIDString = blogger.AddPost(post);
                    //if (!String.IsNullOrEmpty(cmdlet.ArticleIDString) && !quiet) {
                    //    Utils.MsgBox("Success", new Win32Exception(0).Message, MessageBoxImage.Information);
                    //}
                    // get post URL once published
                    if (!String.IsNullOrEmpty(cmdlet.ArticleIDString)) {
                        try {
                            cmdlet.URL = module.Provider.ProviderName.ToLower() == "codeplex"
                                ? module.Provider.Blog.URL + "wikipage?title=" + cmdlet.Name
                                : blogger.GetPost(cmdlet.ArticleIDString).Permalink;
                            if (!Uri.IsWellFormedUriString(cmdlet.URL, UriKind.Absolute)) {
                                var baseUrl = new Uri(module.Provider.ProviderURL);
                                cmdlet.URL = $"{baseUrl.Scheme}://{baseUrl.DnsSafeHost}{cmdlet.URL}";
                            }
                        } catch { }
                    }
                } else {
                    try {
                        // assuming that article exist, so we just change it
                        blogger.UpdatePost(post);
                        var baseUrl = new Uri(module.Provider.ProviderURL);
                        String permalink = blogger.GetPost(cmdlet.ArticleIDString).Permalink;
                        cmdlet.URL = $"{baseUrl.Scheme}://{baseUrl.DnsSafeHost}{permalink}";
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
                Utils.MsgBox("Warning", Strings.WarnBloggerNeedsMoreData, MessageBoxImage.Exclamation);
                return;
            }
            List<CmdletObject> cmdletsToProcess = module.Cmdlets.Where(x => x.Publish).ToList();
            Double duration = 100.0 / cmdletsToProcess.Count;
            pb.Value = 0;
            foreach (CmdletObject cmdlet in cmdletsToProcess) {
                await PublishSingle(cmdlet, module, blogger, true);
                pb.Value += duration;
            }
            Utils.MsgBox("Success", (new Win32Exception(0)).Message, MessageBoxImage.Information);
            pb.Value = 100;
        }
    }
}
