using Unity;

namespace PsCmdletHelpEditor.BLL {
    public static class UnityConfig {

        public static IUnityContainer Container { get; private set; }

        public static void Configure(IUnityContainer container) {
            Container = container;
        }
    }
}
