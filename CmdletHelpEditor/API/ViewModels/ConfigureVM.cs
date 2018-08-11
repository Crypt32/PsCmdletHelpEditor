using System;
using System.Windows;
using System.Windows.Input;
using CmdletHelpEditor.API.Main;
using CmdletHelpEditor.Properties;

namespace CmdletHelpEditor.API.ViewModel {
	class ConfigureVM : DependencyObject {
		public ConfigureVM() {
			Settings.Default.Reload();
			SaveConfigCommand = new RelayCommand(SaveConfig);
		}
		public ICommand SaveConfigCommand { get; set; }

		static void SaveConfig(Object obj) {
			if (Settings.Default.CodePlexTextEnabled && (String.IsNullOrEmpty(Settings.Default.CodeplexProject) || String.IsNullOrWhiteSpace(Settings.Default.CodeplexProject))) {
				Tools.MsgBox("Invalid name", "Project name cannot be empty.", MessageBoxButton.OK, MessageBoxImage.Warning);
			} else {
				Settings.Default.Save();
			}
		}
	}
}
