using System;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace PsCmdletHelpEditor.BLL.Models {
    public class OnlinePublishEntry : ViewModelBase {
        CmdletObject cmdlet;
        OnlinePublishStatusEnum status = OnlinePublishStatusEnum.Pending;
        String statusText = "Pending";

        public CmdletObject Cmdlet {
            get => cmdlet;
            set {
                cmdlet = value;
                OnPropertyChanged(nameof(Cmdlet));
            }
        }
        public OnlinePublishStatusEnum Status {
            get => status;
            set {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        public String StatusText {
            get => statusText;
            set {
                statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }
    }
}
