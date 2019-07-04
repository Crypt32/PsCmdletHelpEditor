using System;
using System.Windows;

namespace PsCmdletHelpEditor.Wpf.Views.Windows {
    /// <summary>
    /// Interaction logic for OnlinePublishProgressWindow.xaml
    /// </summary>
    public partial class OnlinePublishProgressWindow {
        public OnlinePublishProgressWindow(ModuleObject module) {
            InitializeComponent();
            ((OnlinePublishProgressVM)DataContext).SetModule(module);
        }

        void CloseClick(Object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
