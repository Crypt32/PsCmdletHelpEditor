using System;
using System.ComponentModel;
using PsCmdletHelpEditor.BLL.Abstraction;
using PsCmdletHelpEditor.BLL.ViewModels;

namespace PsCmdletHelpEditor.Wpf.Views.Windows {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public MainWindow(IMainWindowVM dataContext) {
            InitializeComponent();
            DataContext = dataContext;
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
