using CmdletHelpEditor.Properties;
using System;
using System.Windows;
using System.Windows.Input;

namespace CmdletHelpEditor.API.ViewModel {
	public class ConfigVM : DependencyObject {
		public ConfigVM() {
			SaveConfigCommand = new RelayCommand(SaveConfig);
		}

		public ICommand SaveConfigCommand { get; set; }

		static void SaveConfig(Object obj) {
			Settings.Default.Save();
		}
	}
}
