using System;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.Models;
class OnlinePublishEntry : ViewModelBase {
    CmdletObject cmdlet;
    OnlinePublishStatusEnum status = OnlinePublishStatusEnum.Pending;
    String statusText = "Pending";

    public CmdletObject Cmdlet {
        get => cmdlet;
        set {
            cmdlet = value;
            OnPropertyChanged();
        }
    }
    public OnlinePublishStatusEnum Status {
        get => status;
        set {
            status = value;
            OnPropertyChanged();
        }
    }
    public String StatusText {
        get => statusText;
        set {
            statusText = value;
            OnPropertyChanged();
        }
    }
}
