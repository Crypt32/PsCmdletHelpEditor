using System;
using System.Windows.Input;

namespace CmdletHelpEditor.Views.UserControls;
/// <summary>
/// Interaction logic for DisplayOutputControl.xaml
/// </summary>
public partial class DisplayOutputControl {
    public DisplayOutputControl() {
        InitializeComponent();
    }

    void onRtbPreviewMouseWheel(Object sender, MouseWheelEventArgs args) {
        if (Keyboard.Modifiers != ModifierKeys.Control) {
            return;
        }

        args.Handled = true;
        if (args.Delta > 0) {
            ++SrcRtbBox.FontSize;
        } else {
            --SrcRtbBox.FontSize;
        }
    }
}
