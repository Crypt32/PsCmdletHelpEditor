using System;
using System.ComponentModel;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    public class ViewModelBase : INotifyPropertyChanged {
        protected void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
