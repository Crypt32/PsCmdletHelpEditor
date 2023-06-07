﻿using System;
using System.Windows;
using System.Windows.Input;
using PsCmdletHelpEditor.BLL.Models;
using PsCmdletHelpEditor.BLL.Tools;
using SysadminsLV.WPF.OfficeTheme.Toolkit;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    public static class MetaWeblogCommands {
        static Boolean working;

        public static ICommand PublishArticleCommand => new RelayCommand(PublishSingle, CanPublish);
        public static ICommand PublishAllCommand => new RelayCommand(PublishAll, CanPublish);

        static async void PublishSingle(Object obj) {
            if (obj == null) { return; }
            working = true;
            var mwvm = (MainWindowVM)Application.Current.MainWindow.DataContext;
            try {
                await MetaWeblogWrapper.PublishSingle((CmdletObject)obj, mwvm.SelectedTab.Module, null, false);
                MsgBox.Show("Success", "The operation completed successfully.", MessageBoxImage.Information);
            } catch (Exception e) {
                MsgBox.Show("Error", e.Message);
            }
            working = false;
        }
        static void PublishAll(Object obj) {
            working = true;
            MainWindowVM mwvm = (MainWindowVM)Application.Current.MainWindow.DataContext;
            (((MainWindow)obj).sb.pb).Visibility = Visibility.Visible;
            MetaWeblogWrapper.PublishAll(mwvm.SelectedTab.Module, ((MainWindow)obj).sb.pb);
            (((MainWindow)obj).sb.pb).Visibility = Visibility.Collapsed;
            working = false;
        }
        static Boolean CanPublish(Object obj) {
            if (working) { return false; }
            MainWindowVM mwvm = (MainWindowVM)Application.Current.MainWindow.DataContext;
            return mwvm.SelectedTab != null &&
                   mwvm.SelectedTab.Module != null &&
                   mwvm.SelectedTab.Module.Provider != null &&
                   (!mwvm.SelectedTab.Module.IsOffline || !mwvm.SelectedTab.Module.UpgradeRequired);
        }
    }
}