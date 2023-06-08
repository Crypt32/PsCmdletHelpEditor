using CmdletHelpEditor.API.Models;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.ViewModels {
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
