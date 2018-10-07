using System;
using System.ComponentModel;

namespace CmdletHelpEditor.API.Models {
    class OnlinePublishEntry : INotifyPropertyChanged {
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

        void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
