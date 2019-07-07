using System;
using System.Windows.Input;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    public abstract class ClosableDialogViewModel : ViewModelBase {
        Boolean? dialogResult;

        protected ClosableDialogViewModel() {
            CloseCommand = new RelayCommand(close);
        }

        public Boolean? DialogResult {
            get => dialogResult;
            set {
                dialogResult = value;
                OnPropertyChanged(nameof(DialogResult));
            }
        }

        public ICommand CloseCommand { get; }

        void close(Object o) {
            DialogResult = true;
        }
    }
}
