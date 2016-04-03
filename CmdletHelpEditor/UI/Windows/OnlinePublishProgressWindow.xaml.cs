using CmdletHelpEditor.API.BaseClasses;
using CmdletHelpEditor.API.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CmdletHelpEditor.UI.Windows {
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
