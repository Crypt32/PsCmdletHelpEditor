using PsCmdletHelpEditor.BLL.Models;

namespace PsCmdletHelpEditor.BLL.Abstraction {
    public interface IOnlinePublishProgressVM {
        void SetModule(PsModuleObject moduleObject);
    }
}