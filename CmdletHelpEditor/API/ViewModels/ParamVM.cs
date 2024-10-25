using CmdletHelpEditor.API.Models;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.ViewModels;
public class ParamVM : ViewModelBase {
    PsCommandParameterVM paramDescription;

    public PsCommandParameterVM CurrentParam {
        get => paramDescription;
        set {
            paramDescription = value;
            OnPropertyChanged();
        }
    }
}
