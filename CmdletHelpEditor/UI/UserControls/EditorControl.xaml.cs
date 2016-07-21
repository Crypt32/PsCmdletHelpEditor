using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.Controls;

namespace CmdletHelpEditor.UI.UserControls {
	/// <summary>
	/// Interaction logic for Editor.xaml
	/// </summary>
	public partial class EditorControl {
		public EditorControl(ClosableModuleItem tab) {
			if (tab != null) {
				DataContext = tab;
			}
			InitializeComponent();
		}
	}
}
