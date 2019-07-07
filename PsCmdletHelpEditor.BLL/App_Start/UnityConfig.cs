using PsCmdletHelpEditor.BLL.Abstraction;
using PsCmdletHelpEditor.BLL.ViewModels;
using Unity;

namespace PsCmdletHelpEditor.BLL {
    public static class UnityConfig {

        public static IUnityContainer Container { get; private set; }

        public static void Configure(IUnityContainer container) {
            Container = container;
            container.RegisterType<IAppConfigVM, AppConfigVM>();
            container.RegisterType<IOnlinePublishProgressVM, OnlinePublishProgressVM>();
        }
    }
}