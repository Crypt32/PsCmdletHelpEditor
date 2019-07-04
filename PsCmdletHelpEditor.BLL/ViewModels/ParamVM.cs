using PsCmdletHelpEditor.BLL.Models;

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
