using System;
using System.Windows.Input;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    public abstract class ClosableDialogViewModel : ViewModelBase {
        Boolean? dialogResult;

        protected ClosableDialogViewModel() {
            CloseCommand = new RelayCommand(CloseView);
        }

        public ICommand CloseCommand { get; }
        public Boolean? DialogResult {
            get => dialogResult;
            set {
                dialogResult = value;
                OnPropertyChanged(nameof(DialogResult));
            }
        }

        protected virtual void CloseView(Object obj) {
            DialogResult = true;
        }
    }
}
