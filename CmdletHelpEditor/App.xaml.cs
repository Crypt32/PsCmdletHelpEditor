using System;
using System.Windows;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.ViewModels;
using CmdletHelpEditor.Views.Windows;
using Unity;

namespace CmdletHelpEditor {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App {

        public App() {
            Container = new UnityContainer();
            configureServices();
        }

        public static IUnityContainer Container { get; private set; }

        void ApplicationStartup(Object sender, StartupEventArgs e) {
            ((App)sender).MainWindow = Container.Resolve<MainWindow>();
            if (e.Args.Length == 1) {
                ((MainWindowVM)((App)sender).MainWindow.DataContext).CommandManager.OpenProject(e.Args[0]);
            }
            ((App)sender).MainWindow.Show();
        }

        void configureServices() {
            Container.RegisterSingleton<IMainWindowVM, MainWindowVM>();
            Container.RegisterSingleton<IDataSource, DataSource>();
        }
    }
}
