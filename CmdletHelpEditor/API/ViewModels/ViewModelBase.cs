using System;
using System.ComponentModel;

namespace CmdletHelpEditor.API.ViewModels {
    class ViewModelBase : INotifyPropertyChanged {
        protected void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
