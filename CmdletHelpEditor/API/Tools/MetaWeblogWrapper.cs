using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CmdletHelpEditor.API.MetaWeblog;
using CmdletHelpEditor.API.Models;

namespace CmdletHelpEditor.API.Tools {
    static class MetaWeblogWrapper {
        public static Task<List<Post<String>>> GetRecentPosts(Blogger blogger, Int32 fetchPostCount) {
            return Task<List<Post<String>>>.Factory.StartNew(() => blogger.GetRecentPosts(fetchPostCount));
        }
        public static Task<List<WpGetPost>> GetPages(Blogger blogger, Int32 fetchPostCount) {
            return Task<List<WpGetPost>>.Factory.StartNew(() => blogger.GetPages(fetchPostCount));
        }
        public static Task PublishSingle(CmdletObject cmdlet, ModuleObject module, Blogger blogger, Boolean quiet) {
            return Task.Factory.StartNew(() => {
                Thread.Sleep(5000);
                if (blogger == null) {
                    blogger = Utils.InitializeBlogger(module.Provider);
                }
                if (blogger == null) {
                    throw new Exception(Strings.WarnBloggerNeedsMoreData);
                }
                if (String.IsNullOrEmpty(cmdlet.ArticleIDString)) {
                    var post = new WpPostCreate {
                        Title = cmdlet.Name,
                        PageName = cmdlet.Name,
                        PostType = "page",
                        PostParent = 16520, // 70 -- mtmsoftware.com, 16520 -- pkisolutions.com
                        HTML = HtmlProcessor.GenerateHtmlView(cmdlet, module).Result
                    };
                    // assuming that article does not exist
                    cmdlet.ArticleIDString = blogger.AddWpPost(post);
                } else {
                    var post = new WpPostUpdate {
                        Title = cmdlet.Name,
                        PostType = "page",
                        PostParent = 16520,
                        HTML = HtmlProcessor.GenerateHtmlView(cmdlet, module).Result
                    };
                    try {
                        // assuming that article exist, so we just change it
                        blogger.UpdateWpPost(post, cmdlet.ArticleIDString);
                    } catch (Exception e) {
                        // 0x80131600 connect succeeds, but the post is deleted. Remove postid
                        if (e.HResult == -2146232832 || e.HResult == -2147023728) {
                            cmdlet.ArticleIDString = null;
                            cmdlet.URL = null;
                        }
                        throw;
                    }
                }
                // get post URL once published
                //if (!String.IsNullOrEmpty(cmdlet.ArticleIDString)) {
                //    try {
                //        cmdlet.URL = blogger.GetWpPost(cmdlet.ArticleIDString).Permalink;
                //        if (!Uri.IsWellFormedUriString(cmdlet.URL, UriKind.Absolute)) {
                //            var baseUrl = new Uri(module.Provider.ProviderURL);
                //            cmdlet.URL = $"{baseUrl.Scheme}://{baseUrl.DnsSafeHost}{cmdlet.URL}";
                //        }
                //    } catch (Exception ex) { }
                //}
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
