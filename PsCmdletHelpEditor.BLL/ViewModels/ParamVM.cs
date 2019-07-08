using PsCmdletHelpEditor.BLL.Models;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    public class ParamVM : ViewModelBase {
        ParameterDescription paramDescription;

        public ParameterDescription CurrentParam {
            get => paramDescription;
            set {
                paramDescription = value;
                OnPropertyChanged(nameof(CurrentParam));
            }
        }
    }
}
