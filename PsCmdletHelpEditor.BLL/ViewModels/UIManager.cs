using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PsCmdletHelpEditor.BLL.Abstraction;
using PsCmdletHelpEditor.BLL.Models;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using Unity;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    public static class UIManager {
        public static ICommand AboutCommand => new RelayCommand(ShowAbout);
        public static ICommand ConfigureCommand => new RelayCommand(ShowConfigure);
        public static ICommand ModulePropsCommand => new RelayCommand(ShowModuleProperties, CanShowModuleProperties);

        public static void ShowAbout(Object obj) {
            var vm = UnityConfig.Container.Resolve<IAboutDialogVM>();
            var uiManager = UnityConfig.Container.Resolve<IUIWindowDialogService>();
            uiManager.ShowDialog("About", vm);
        }
        public static void ShowConfigure(Object obj) {
            var uiManager = UnityConfig.Container.Resolve<IUIWindowDialogService>();
            var vm = UnityConfig.Container.Resolve<IAppConfigVM>();
            uiManager.ShowDialog("Settings", vm);
        }
        public static void ShowModuleProperties(Object obj) {
            Window mainWindow = Application.Current.MainWindow;
            var ModulePropsDlg = new ModuleProperties((MainWindowVM)mainWindow?.DataContext) {
                Owner = mainWindow
            };
            ModulePropsDlg.ShowDialog();
        }

        static Boolean CanShowModuleProperties(Object obj) {
            return (obj as ClosableModuleItem)?.Module != null;
        }
        public static ClosableModuleItem GenerateTab() {
            var cti = new ClosableModuleItem {
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
            var mwvm = (MainWindowVM)Application.Current.MainWindow.DataContext;
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
