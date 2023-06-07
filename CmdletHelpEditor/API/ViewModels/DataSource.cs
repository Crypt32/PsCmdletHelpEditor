using System.Collections.ObjectModel;
using CmdletHelpEditor.Abstract;

namespace CmdletHelpEditor.API.ViewModels {
    class DataSource : ViewModelBase, IDataSource {


        public ObservableCollection<PsModuleItem> ModuleList { get; }
            = new ObservableCollection<PsModuleItem>();
        public PsModuleItem ActiveModule { get; set; }
    }
}
