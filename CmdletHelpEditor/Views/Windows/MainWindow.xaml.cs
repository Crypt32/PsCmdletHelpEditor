using System;
using System.ComponentModel;
using CmdletHelpEditor.API.Tools;
using CmdletHelpEditor.API.ViewModels;
using CmdletHelpEditor.Properties;

namespace CmdletHelpEditor.Views.Windows {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();
        }

        void WindowClosing(Object sender, CancelEventArgs e) {
            ((MainWindowVM)DataContext).CommandManager.CloseAppCommand.Execute(e);
        }
        async void WindowInitialized(Object sender, EventArgs e) {
            ((MainWindowVM)DataContext).PsVersion = await PowerShellProcessor.GetPsVersion();
            Settings.Default.Reload();
        }
    }
}
