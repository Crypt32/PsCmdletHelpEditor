using System;
using System.Windows.Input;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace CmdletHelpEditor.API.ViewModels {
    public abstract class ClosableWindowVM : ViewModelBase {
        Boolean? dialogResult;

        protected ClosableWindowVM() {
            CloseCommand = new RelayCommand(o => { DialogResult = true; });
        }

        public ICommand CloseCommand { get; }

        public Boolean? DialogResult {
            get => dialogResult;
            set {
                dialogResult = value;
                OnPropertyChanged(nameof(DialogResult));
            }
        }
    }
}