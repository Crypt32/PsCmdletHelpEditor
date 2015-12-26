using System.Windows;

namespace CmdletHelpEditor.UI.Dialogs {
	/// <summary>
	/// Interaction logic for Configure.xaml
	/// </summary>
	public partial class Options {
		public Options() {
			InitializeComponent();
		}

		void ButtonClose(object sender, RoutedEventArgs e) {
			Close();
		}
	}
}
