using System;
using System.ComponentModel;
using CmdletHelpEditor.API.Models;

namespace CmdletHelpEditor.API.ViewModels {
    public class ParamVM : INotifyPropertyChanged {
        ParameterDescription paramDescription;

        public ParameterDescription CurrentParam {
            get => paramDescription;
            set {
                paramDescription = value;
                OnPropertyChanged(nameof(CurrentParam));
            }
        }
        
        void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
