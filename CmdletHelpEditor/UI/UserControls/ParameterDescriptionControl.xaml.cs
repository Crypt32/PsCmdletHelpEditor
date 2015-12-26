using CmdletHelpEditor.API.BaseClasses;
using CmdletHelpEditor.API.ViewModel;
using CmdletHelpEditor.Controls;
using System.Windows.Controls;

namespace CmdletHelpEditor.UI.UserControls {
	/// <summary>
	/// Interaction logic for ParameterDescriptionControl.xaml
	/// </summary>
	public partial class ParameterDescriptionControl {
		public ParameterDescriptionControl() {
			InitializeComponent();
		}
		void lvParametersSelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (DataContext == null) { return; }
			ParamVM vm = ((ClosableTabItem)DataContext).EditorContext.ParamContext;
			vm.CurrentParam = (ParameterDescription)lvParameters.SelectedItem;
			vm.ParamTextBoxEnabled = lvParameters.SelectedIndex >= 0;
		}
	}
}
