using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using PsCmdletHelpEditor.Core.Services.Formatters;
using PsCmdletHelpEditor.XmlRpc;
using PsCmdletHelpEditor.XmlRpc.WordPress;
using Unity;

namespace CmdletHelpEditor.API.Tools;

static class MetaWeblogWrapper {
    public static async Task PublishSingle(CmdletObject cmdlet, ModuleObject module, WpXmlRpcClient blogger) {
        await Task.Factory.StartNew(() => Thread.Sleep(5000));
        blogger ??= module.Provider.InitializeBlogger();
        if (blogger == null) {
            throw new Exception(Strings.WarnBloggerNeedsMoreData);
        }
        var htmlGenerator = OutputFormatterFactory.GetHtmlFormatter();
        if (String.IsNullOrEmpty(cmdlet.ArticleIDString)) {
            var post = new WpPostCreate {
                Title = cmdlet.Name,
                PageName = cmdlet.Name,
                PostType = "page",
                PostParent = 16520,
                HTML = await htmlGenerator.GenerateViewAsync(cmdlet.ToXmlObject(), module.ToXmlObject())
            };
            // assuming that article does not exist
            cmdlet.ArticleIDString = await blogger.AddPostAsync(post);
        } else {
            var post = new WpPostUpdate {
                Title = cmdlet.Name,
                PostType = "page",
                PostParent = 16520,
                HTML = await htmlGenerator.GenerateViewAsync(cmdlet.ToXmlObject(), module.ToXmlObject())
            };
            try {
                // assuming that article exist, so we just change it
                await blogger.UpdatePostAsync(post, Convert.ToInt32(cmdlet.ArticleIDString));
            } catch (Exception e) {
                // 0x80131600 connect succeeds, but the post is deleted. Remove postid
                if (e.HResult is -2146232832 or -2147023728) {
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
    public static async Task PublishAll(ModuleObject module, IProgressBar pb) {
        IUIMessenger uiMessenger = App.Container.Resolve<IUIMessenger>();
        WpXmlRpcClient blogger = module.Provider.InitializeBlogger();
        if (blogger == null) {
            uiMessenger.ShowWarning("Warning", Strings.WarnBloggerNeedsMoreData);
            return;
        }
        List<CmdletObject> cmdletsToProcess = module.Cmdlets.Where(x => x.Publish).ToList();
        Double duration = 100.0 / cmdletsToProcess.Count;
        pb.Progress = 0;
        foreach (CmdletObject cmdlet in cmdletsToProcess) {
            await PublishSingle(cmdlet, module, blogger);
            pb.Progress += duration;
        }

        uiMessenger.ShowInformation("Success", new Win32Exception(0).Message);
        pb.Progress = 100;
    }
}