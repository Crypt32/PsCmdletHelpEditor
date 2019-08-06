using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PsCmdletHelpEditor.BLL.Models;
using PsCmdletHelpEditor.XmlRpc;
using SysadminsLV.WPF.OfficeTheme.Toolkit;

namespace PsCmdletHelpEditor.BLL.Tools {
    public static class MetaWeblogWrapper {
        public static async Task PublishSingle(CmdletObject cmdlet, ModuleObject module, IXmlRpcClient blogger, Boolean quiet) {
            var post = new XmlRpcPost {
                Title = cmdlet.Name,
                PostId = cmdlet.ArticleIDString,
                HTML = HtmlProcessor.GenerateHtmlView(cmdlet, module).Result
            };
            if (blogger == null) {
                blogger = Utils.InitializeBlogger(module.Provider);
            }
            if (blogger == null) {
                MsgBox.Show("Warning", Strings.WarnBloggerNeedsMoreData, MessageBoxImage.Exclamation);
                return;
            }
            if (String.IsNullOrEmpty(cmdlet.ArticleIDString)) {
                // assuming that article does not exist
                cmdlet.ArticleIDString = await blogger.AddPostAsync(post);
                if (!String.IsNullOrEmpty(cmdlet.ArticleIDString) && !quiet) {
                    MsgBox.Show("Success", new Win32Exception(0).Message, MessageBoxImage.Information);
                }
                // get post URL once published
                if (!String.IsNullOrEmpty(cmdlet.ArticleIDString)) {
                    try {
                        cmdlet.URL = module.Provider.ProviderName.ToLower() == "codeplex"
                            ? module.Provider.Blog.URL + "wikipage?title=" + cmdlet.Name
                            : (await blogger.GetPostAsync(cmdlet.ArticleIDString)).Permalink;
                        if (!Uri.IsWellFormedUriString(cmdlet.URL, UriKind.Absolute)) {
                            var baseUrl = new Uri(module.Provider.ProviderURL);
                            cmdlet.URL = $"{baseUrl.Scheme}://{baseUrl.DnsSafeHost}{cmdlet.URL}";
                        }
                    } catch { }
                }
            } else {
                try {
                    // assuming that article exist, so we just change it
                    await blogger.UpdatePostAsync(post);
                    var baseUrl = new Uri(module.Provider.ProviderURL);
                    String permalink = (await blogger.GetPostAsync(cmdlet.ArticleIDString)).Permalink;
                    cmdlet.URL = $"{baseUrl.Scheme}://{baseUrl.DnsSafeHost}{permalink}";
                } catch (Exception e) {
                    // 0x80131600 connect succeeds, but the post is deleted. Remove postid
                    if (e.HResult == -2146232832 || e.HResult == -2147023728) {
                        cmdlet.ArticleIDString = null;
                    }
                    throw;
                }
            }
        }
        public static async void PublishAll(ModuleObject module, ProgressBar pb) {
            IXmlRpcClient blogger = Utils.InitializeBlogger(module.Provider);
            if (blogger == null) {
                MsgBox.Show("Warning", Strings.WarnBloggerNeedsMoreData, MessageBoxImage.Exclamation);
                return;
            }
            List<CmdletObject> cmdletsToProcess = module.Cmdlets.Where(x => x.Publish).ToList();
            Double duration = 100.0 / cmdletsToProcess.Count;
            pb.Value = 0;
            foreach (CmdletObject cmdlet in cmdletsToProcess) {
                await PublishSingle(cmdlet, module, blogger, true);
                pb.Value += duration;
            }
            MsgBox.Show("Success", (new Win32Exception(0)).Message, MessageBoxImage.Information);
            pb.Value = 100;
        }
    }
}
