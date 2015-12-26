using CmdletHelpEditor.API.BaseClasses;
using CmdletHelpEditor.API.ViewModel;
using System;
using System.Windows.Input;

namespace CmdletHelpEditor.UI.UserControls {
	/// <summary>
	/// Interaction logic for ModuleSelectorControl.xaml
	/// </summary>
	public partial class ModuleSelectorControl {
		public ModuleSelectorControl() {
			InitializeComponent();
		}
		
		void lvModulesMouseDoubleClick(Object sender, MouseButtonEventArgs e) {
			((MainWindowVM)DataContext).CommandManager.LoadCmdlets(null, false);
		}
	}
}
