using System.Collections.ObjectModel;
using CmdletHelpEditor.Abstract;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.ViewModels {
    class DataSource : ViewModelBase, IDataSource {


        public ObservableCollection<PsModuleItem> ModuleList { get; }
            = new ObservableCollection<PsModuleItem>();
        public PsModuleItem ActiveModule { get; set; }
    }
}
