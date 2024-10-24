using System;
using System.ComponentModel;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.ViewModels;
using CmdletHelpEditor.Properties;
using PsCmdletHelpEditor.Core.Services;

namespace CmdletHelpEditor.Views.Windows;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow {
    readonly IPowerShellProcessor _psProcessor;

    public MainWindow(IMainWindowVM dataContext, IPowerShellProcessor psProcessor) {
        DataContext = dataContext;
        _psProcessor = psProcessor;
        InitializeComponent();
    }

    void WindowClosing(Object sender, CancelEventArgs e) {
        ((MainWindowVM)DataContext).CommandManager.CloseAppCommand.Execute(e);
    }
    async void WindowInitialized(Object sender, EventArgs e) {
        var dc = (MainWindowVM)DataContext;
        dc.PsVersion = await _psProcessor.GetPsVersionAsync();
        Settings.Default.Reload();
    }
}
