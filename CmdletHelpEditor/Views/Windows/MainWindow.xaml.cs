using System;
using System.ComponentModel;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Tools;
using CmdletHelpEditor.API.ViewModels;
using CmdletHelpEditor.Properties;

namespace CmdletHelpEditor.Views.Windows {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public MainWindow(IMainWindowVM dataContext) {
            DataContext = dataContext;
            InitializeComponent();
        }

        void WindowClosing(Object sender, CancelEventArgs e) {
            ((MainWindowVM)DataContext).CommandManager.CloseAppCommand.Execute(e);
        }
        async void WindowInitialized(Object sender, EventArgs e) {
            var dc = (MainWindowVM)DataContext;
            dc.PsVersion = await PowerShellProcessor.GetPsVersion();
            Settings.Default.Reload();
        }
    }
}
