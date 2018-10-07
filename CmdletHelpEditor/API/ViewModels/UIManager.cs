using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.Properties;
using CmdletHelpEditor.Views.Dialogs;
using CmdletHelpEditor.Views.UserControls;
using CmdletHelpEditor.Views.UserControls.Parts;
using CmdletHelpEditor.Views.Windows;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace CmdletHelpEditor.API.ViewModels {
    public static class UIManager {
        public static ICommand AboutCommand => new RelayCommand(ShowAbout);
        public static ICommand ConfigureCommand => new RelayCommand(ShowConfigure);
        public static ICommand ModulePropsCommand => new RelayCommand(ShowModuleProperties, CanShowModuleProperties);

        public static ICommand ShowToolBarCommand => new RelayCommand(ShowToolBar);
        public static ICommand ShowStatusBarCommand => new RelayCommand(ShowStatusBar);

        public static void ShowAbout(Object obj) {
            AboutBox AboutDlg = new AboutBox(Application.Current.Windows[0]);
            AboutDlg.ShowDialog();
        }
        public static void ShowConfigure(Object obj) {
            Options OptDlg = new Options();
            OptDlg.ShowDialog();
        }
        public static void ShowModuleProperties(Object obj) {
            ModuleProperties ModulePropsDlg = new ModuleProperties((MainWindowVM)Application.Current.MainWindow.DataContext);
            ModulePropsDlg.ShowDialog();
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
            return (obj as ClosableModuleItem)?.Module != null;
        }
        public static ClosableModuleItem GenerateTab() {
            ClosableModuleItem cti = new ClosableModuleItem {
                Header = "untitled",
                IsSaved = true,
                IsClosable = true,
            };
            Panel content = new Grid();
            content.Children.Add(new StartUserControl());
            cti.Content = content;
            cti.EditorContext = new EditorVM(cti);
            return cti;
        }
        public static void ShowBusy(ContentControl tab, String text) {
            ((Grid)tab.Content).Children.Clear();
            ((Grid)tab.Content).Children.Add(new BusyUC(text));
        }
        public static void ShowModuleList(ContentControl tab) {
            ((Grid)tab.Content).Children.Clear();
            ((Grid)tab.Content).Children.Add(new ModuleSelectorControl());
        }
        public static void ShowEditor(ClosableModuleItem tab) {
            MainWindowVM mwvm = (MainWindowVM)Application.Current.MainWindow.DataContext;
            mwvm.SelectedModule = null;
            ((Grid)tab.Content).Children.Clear();
            ((Grid)tab.Content).Children.Add(new EditorControl(tab));
        }
        public static void RestoreControl(ContentControl tab, UIElement element) {
            ((Grid)tab.Content).Children.Clear();
            ((Grid)tab.Content).Children.Add(element);
        }
    }
}
