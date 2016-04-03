using System;
using System.ComponentModel;

namespace CmdletHelpEditor.API.BaseClasses {
    class OnlinePublishEntry : INotifyPropertyChanged {
        CmdletObject cmdlet;
        OnlinePublishStatusEnum status = OnlinePublishStatusEnum.Pending;
        String statusText = "Pending";

        public CmdletObject Cmdlet {
            get { return cmdlet; }
            set {
                cmdlet = value;
                OnPropertyChanged("Cmdlet");
            }
        }
        public OnlinePublishStatusEnum Status {
            get { return status; }
            set {
                status = value;
                OnPropertyChanged("Status");
            }
        }
        public String StatusText {
            get { return statusText; }
            set {
                statusText = value;
                OnPropertyChanged("StatusText");
            }
        }

        void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
