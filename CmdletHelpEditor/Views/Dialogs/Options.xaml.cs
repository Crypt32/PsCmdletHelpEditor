using System;
using System.Windows;

namespace CmdletHelpEditor.Views.Dialogs {
    /// <summary>
    /// Interaction logic for Configure.xaml
    /// </summary>
    public partial class Options {
        public Options() {
            InitializeComponent();
        }

        void ButtonClose(Object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
