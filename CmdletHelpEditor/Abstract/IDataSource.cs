using System.Collections.ObjectModel;
using CmdletHelpEditor.API.ViewModels;

namespace CmdletHelpEditor.Abstract {
    public interface IDataSource {
        ObservableCollection<PsModuleItem> ModuleList { get; }
        PsModuleItem ActiveModule { get; set; }
    }
}