using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.API.Tools;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using Unity;

namespace CmdletHelpEditor.API.ViewModels;

public static class MetaWeblogCommands {
    static Boolean working;

    public static IAsyncCommand PublishArticleCommand => new AsyncCommand(publishSingle, canPublish);
    public static IAsyncCommand PublishAllCommand => new AsyncCommand(publishAll, canPublish);

    static async Task publishSingle(Object obj, CancellationToken token = default) {
        if (obj == null) {
            return;
        }
        IUIMessenger uiMessenger = App.Container.Resolve<IUIMessenger>();
        working = true;
        var mwvm = (MainWindowVM)Application.Current.MainWindow.DataContext;
        try {
            await MetaWeblogWrapper.PublishSingle((CmdletObject)obj, ((HelpProjectDocument)mwvm.SelectedDocument)!.Module, null);
            uiMessenger.ShowInformation("Success", "The operation completed successfully.");
        } catch (Exception e) {
            uiMessenger.ShowError("Error", e.Message);
        }
        working = false;
    }
    static async Task publishAll(Object obj, CancellationToken token = default) {
        working = true;
        IProgressBar pb = App.Container.Resolve<IProgressBar>();
        var mwvm = (MainWindowVM)Application.Current.MainWindow.DataContext;
        pb.Start();
        await MetaWeblogWrapper.PublishAll(((HelpProjectDocument)mwvm.SelectedDocument)!.Module, pb);
        pb.End();
        working = false;
    }
    static Boolean canPublish(Object obj) {
        if (working) {
            return false;
        }
        var mwvm = (MainWindowVM)Application.Current.MainWindow.DataContext;
        return mwvm.SelectedDocument is HelpProjectDocument { Module.Provider: not null } helpProject
               && (!helpProject.Module.IsOffline || !helpProject.Module.UpgradeRequired);
    }
}