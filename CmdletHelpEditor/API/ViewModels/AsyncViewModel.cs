using System;
using CmdletHelpEditor.Abstract;

namespace CmdletHelpEditor.API.ViewModels {
    public abstract class AsyncViewModel : ViewModelBase, IAsyncVM {
        Boolean isBusy;

        public Boolean IsBusy {
            get => isBusy;
            set {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }
    }
}