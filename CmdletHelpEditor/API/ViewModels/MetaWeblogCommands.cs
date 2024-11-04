using System;
using System.Windows;
using System.Windows.Input;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.API.Tools;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using Unity;

namespace CmdletHelpEditor.API.ViewModels;

public static class MetaWeblogCommands {
    static Boolean working;

    public static ICommand PublishArticleCommand => new RelayCommand(publishSingle, canPublish);
    public static ICommand PublishAllCommand => new RelayCommand(publishAll, canPublish);

    static async void publishSingle(Object obj) {
        if (obj == null) {
            return;
        }
        IMsgBox msgBox = App.Container.Resolve<IMsgBox>();
        working = true;
        var mwvm = (MainWindowVM)Application.Current.MainWindow.DataContext;
        try {
            await MetaWeblogWrapper.PublishSingle((CmdletObject)obj, ((HelpProjectDocument)mwvm.SelectedDocument)!.Module, null);
            msgBox.ShowInfo("Success", "The operation completed successfully.");
        } catch (Exception e) {
            msgBox.ShowError("Error", e.Message);
        }
        working = false;
    }
    static void publishAll(Object obj) {
        working = true;
        IProgressBar pb = App.Container.Resolve<IProgressBar>();
        var mwvm = (MainWindowVM)Application.Current.MainWindow.DataContext;
        pb.Start();
        MetaWeblogWrapper.PublishAll(((HelpProjectDocument)mwvm.SelectedDocument)!.Module, pb);
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