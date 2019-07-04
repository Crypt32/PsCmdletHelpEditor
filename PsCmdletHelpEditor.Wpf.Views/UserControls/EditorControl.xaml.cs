using CmdletHelpEditor.API.Models;

namespace PsCmdletHelpEditor.Wpf.Views.UserControls {
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
