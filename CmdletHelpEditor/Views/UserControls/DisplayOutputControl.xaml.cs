using System.Windows.Input;

namespace CmdletHelpEditor.Views.UserControls {
    /// <summary>
    /// Interaction logic for DisplayOutputControl.xaml
    /// </summary>
    public partial class DisplayOutputControl {
        public DisplayOutputControl() {
            InitializeComponent();
        }

        void onRtbPreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            if (Keyboard.Modifiers != ModifierKeys.Control) {
                return;
            }

            e.Handled = true;
            if (e.Delta > 0) {
                ++SrcRtbBox.FontSize;
            } else {
                --SrcRtbBox.FontSize;
            }
        }
    }
}
