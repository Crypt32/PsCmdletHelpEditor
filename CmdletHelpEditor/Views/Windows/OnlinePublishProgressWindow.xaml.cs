using System;
using System.Windows;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.API.ViewModels;

namespace CmdletHelpEditor.Views.Windows {
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
