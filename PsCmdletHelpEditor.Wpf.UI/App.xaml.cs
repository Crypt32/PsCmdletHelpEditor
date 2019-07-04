using System.Windows;
using PsCmdletHelpEditor.BLL;
using Unity;

namespace PsCmdletHelpEditor.Wpf.UI {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App {
        public App() {
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            IUnityContainer container = new UnityContainer();
            UnityConfig.Configure(container);
        }

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
            Current.Shutdown(0);
        }
    }
}
