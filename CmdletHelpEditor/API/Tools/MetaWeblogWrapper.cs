using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CmdletHelpEditor.API.Models;
using PsCmdletHelpEditor.XmlRpc;
using PsCmdletHelpEditor.XmlRpc.WordPress;
using SysadminsLV.WPF.OfficeTheme.Toolkit;

namespace CmdletHelpEditor.API.Tools {
    static class MetaWeblogWrapper {
        public static async Task PublishSingle(CmdletObject cmdlet, ModuleObject module, WpXmlRpcClient blogger) {
            await Task.Factory.StartNew(() => Thread.Sleep(5000));
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
                    HTML = await HtmlProcessor.GenerateHtmlView(cmdlet, module)
                };
                // assuming that article does not exist
                cmdlet.ArticleIDString = await blogger.AddPostAsync(post);
            } else {
                var post = new WpPostUpdate {
                    Title = cmdlet.Name,
                    PostType = "page",
                    PostParent = 16520,
                    HTML = await HtmlProcessor.GenerateHtmlView(cmdlet, module)
                };
                try {
                    // assuming that article exist, so we just change it
                    await blogger.UpdatePostAsync(post, Convert.ToInt32(cmdlet.ArticleIDString));
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
        }
        public static async void PublishAll(ModuleObject module, ProgressBar pb) {
            WpXmlRpcClient blogger = Utils.InitializeBlogger(module.Provider);
            if (blogger == null) {
                MsgBox.Show("Warning", Strings.WarnBloggerNeedsMoreData, MessageBoxImage.Exclamation);
                return;
            }
            List<CmdletObject> cmdletsToProcess = module.Cmdlets.Where(x => x.Publish).ToList();
            Double duration = 100.0 / cmdletsToProcess.Count;
            pb.Value = 0;
            foreach (CmdletObject cmdlet in cmdletsToProcess) {
                await PublishSingle(cmdlet, module, blogger);
                pb.Value += duration;
            }

            MsgBox.Show("Success", new Win32Exception(0).Message, MessageBoxImage.Information);
            pb.Value = 100;
        }
    }
}
