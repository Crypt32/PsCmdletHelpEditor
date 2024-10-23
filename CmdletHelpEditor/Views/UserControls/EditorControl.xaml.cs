using CmdletHelpEditor.API.Models;

namespace CmdletHelpEditor.Views.UserControls;
/// <summary>
/// Interaction logic for Editor.xaml
/// </summary>
public partial class EditorControl {
    public EditorControl() {
        InitializeComponent();
    }
    public EditorControl(ClosableModuleItem tab) {
        if (tab != null) {
            DataContext = tab;
        }
        InitializeComponent();
    }
}
