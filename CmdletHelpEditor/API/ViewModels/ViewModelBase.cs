using System;
using System.ComponentModel;

namespace CmdletHelpEditor.API.ViewModels {
    public class ViewModelBase : INotifyPropertyChanged {
        protected void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
