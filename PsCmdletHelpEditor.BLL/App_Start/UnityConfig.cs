using PsCmdletHelpEditor.BLL.Abstraction;
using PsCmdletHelpEditor.BLL.ViewModels;
using Unity;

namespace PsCmdletHelpEditor.BLL {
    public static class UnityConfig {

        public static IUnityContainer Container { get; private set; }

        public static void Configure(IUnityContainer container) {
            Container = container;
            Container.RegisterSingleton<IMainWindowVM, MainWindowVM>();
            container.RegisterSingleton<IAppConfigVM, AppConfigVM>();
            container.RegisterSingleton<IFormatCommands, FormatCommands>();
            container.RegisterType<IOnlinePublishProgressVM, OnlinePublishProgressVM>();
        }
    }
}