using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CmdletHelpEditor.Properties;
using CmdletHelpEditor.Views.Dialogs;
using CmdletHelpEditor.Views.UserControls.Parts;
using CmdletHelpEditor.Views.Windows;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace CmdletHelpEditor.API.ViewModels;
public static class UIManager {
    public static ICommand AboutCommand => new RelayCommand(ShowAbout);
    public static ICommand ConfigureCommand => new RelayCommand(ShowConfigure);
    public static ICommand ModulePropsCommand => new RelayCommand(ShowModuleProperties, CanShowModuleProperties);

    public static ICommand ShowToolBarCommand => new RelayCommand(ShowToolBar);
    public static ICommand ShowStatusBarCommand => new RelayCommand(ShowStatusBar);

    public static void ShowAbout(Object obj) {
        var AboutDlg = new AboutBox(Application.Current.MainWindow);
        AboutDlg.ShowDialog();
    }
    public static void ShowConfigure(Object obj) {
        Window mainWindow = Application.Current.MainWindow;
        var OptDlg = new Options {
            Owner = mainWindow
        };
        OptDlg.ShowDialog();
    }
    public static void ShowModuleProperties(Object obj) {
        Window mainWindow = Application.Current.MainWindow;
        if (((MainWindowVM)mainWindow!.DataContext).SelectedDocument is HelpProjectDocument helpProject) {
            var ModulePropsDlg = new ModuleProperties(helpProject) {
                Owner = mainWindow
            };
            ModulePropsDlg.ShowDialog();
        }
    }

    static void ShowToolBar(Object obj) {
        ToolBarTray tbt = ((MainWindow)obj).tbt;
        tbt.Visibility = tbt.Visibility == Visibility.Visible
            ? Visibility.Collapsed
            : Visibility.Visible;
        Settings.Default.ToolbarChecked = tbt.Visibility == Visibility.Visible;
    }
    static void ShowStatusBar(Object obj) {
        StatusBarMain sb = ((MainWindow)obj).sb;
        sb.Visibility = sb.Visibility == Visibility.Visible
            ? Visibility.Collapsed
            : Visibility.Visible;
        Settings.Default.StatusbarChecked = sb.Visibility == Visibility.Visible;
    }

    static Boolean CanShowModuleProperties(Object obj) {
        return (obj as HelpProjectDocument)?.Module is not null;
    }
}
