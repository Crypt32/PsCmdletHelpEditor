using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.API.Tools;
using PsCmdletHelpEditor.XmlRpc;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace CmdletHelpEditor.API.ViewModels;

public interface IOnlinePublishProgressVM {
    void SetModule(ModuleObject moduleObject);
}
class OnlinePublishProgressVM : AsyncViewModel, IOnlinePublishProgressVM {
    readonly IUIMessenger _uiMessenger;

    Boolean stopRequested;
    String publishCaption;
    ModuleObject module;
    OnlinePublishEntry selectedEntry;

    public OnlinePublishProgressVM(IProgressBar progressBar, IUIMessenger uiMessenger) {
        _uiMessenger = uiMessenger;
        ProgressBar = progressBar;
        PublishCommand = new AsyncCommand(publish);
        RetryCommand = new AsyncCommand(retry);
        PublishCaption = "Publish";
    }

    public IAsyncCommand PublishCommand { get; }
    public IAsyncCommand RetryCommand { get; }

    public ObservableCollection<OnlinePublishEntry> Cmdlets { get; } = new();

    public OnlinePublishEntry SelectedEntry {
        get => selectedEntry;
        set {
            selectedEntry = value;
            OnPropertyChanged(nameof(SelectedEntry));
        }
    }

    public String PublishCaption {
        get => publishCaption;
        set {
            publishCaption = value;
            OnPropertyChanged(nameof(PublishCaption));
        }
    }

    public IProgressBar ProgressBar { get; }

    async Task publish(IScrollToView lv, ICollection<OnlinePublishEntry> cmdlets) {
        WpXmlRpcClient blogger = module.Provider.InitializeBlogger();
        if (blogger == null) {
            _uiMessenger.ShowWarning("Warning", Strings.WarnBloggerNeedsMoreData);
            return;
        }
        Double duration = 100.0 / cmdlets.Count;
        ProgressBar.Start();
        StartSpinner();
        PublishCaption = "Stop";
        foreach (OnlinePublishEntry cmdlet in cmdlets) {
            if (stopRequested) {
                break;
            }
            lv.ScrollIntoView(cmdlet);
            if (cmdlet.Cmdlet.Publish) {
                try {
                    await MetaWeblogWrapper.PublishSingle(cmdlet.Cmdlet, module, blogger);
                    cmdlet.Status = OnlinePublishStatusEnum.Succeed;
                    cmdlet.StatusText = "The operation completed successfully.";
                } catch (Exception e) {
                    cmdlet.Status = OnlinePublishStatusEnum.Failed;
                    cmdlet.StatusText = e.Message;
                }
            } else {
                cmdlet.Status = OnlinePublishStatusEnum.Skipped;
                cmdlet.StatusText = "The item is not configured for publishing";
            }

            ProgressBar.Progress += duration;
        }
        ProgressBar.End();
        StopSpinner();
        PublishCaption = "Publish";
    }
    async Task retry(Object o, CancellationToken token) {
        if (IsBusy) {
            return;
        }
        var lv = (IScrollToView)o;
        await publish(lv, Cmdlets.Where(x => x.Status == OnlinePublishStatusEnum.Failed).ToList());
    }
    async Task publish(Object o, CancellationToken token) {
        if (IsBusy) {
            stopRequested = true;
            return;
        }
        var lv = (IScrollToView)o;
        foreach (OnlinePublishEntry cmdlet in Cmdlets) {
            cmdlet.Status = OnlinePublishStatusEnum.Pending;
            cmdlet.StatusText = "Pending for publish";
        }
        await publish(lv, Cmdlets);
    }

    public void SetModule(ModuleObject moduleObject) {
        module = moduleObject;
        foreach (CmdletObject cmdlet in module.Cmdlets) {
            Cmdlets.Add(new OnlinePublishEntry { Cmdlet = cmdlet });
        }
    }
}