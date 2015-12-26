using CmdletHelpEditor.API.BaseClasses;
using CmdletHelpEditor.API.Tools;
using CmdletHelpEditor.UI.Windows;
using System;
using System.Windows;
using System.Windows.Input;

namespace CmdletHelpEditor.API.ViewModel {
	public static class MetaWeblogCommands {
		static Boolean working;

		public static ICommand PublishArticleCommand {
			get { return new RelayCommand(PublishSingle, CanPublish); }
		}
		public static ICommand PublishAllCommand {
			get { return new RelayCommand(PublishAll, CanPublish); }
		}

		async static void PublishSingle(Object obj) {
			if (obj == null) { return; }
			working = true;
			MainWindowVM mwvm = (MainWindowVM)Application.Current.MainWindow.DataContext;
			await MetaWeblogWrapper.PublishSingle((CmdletObject)obj, mwvm.SelectedTab.Module, null, false);
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
