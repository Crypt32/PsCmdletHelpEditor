using System;
using System.Windows.Input;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace CmdletHelpEditor.API.ViewModels;
public abstract class ClosableWindowVM : AsyncViewModel {
    Boolean? dialogResult;

    protected ClosableWindowVM() {
        CloseCommand = new RelayCommand(_ => { DialogResult = true; });
    }

    public ICommand CloseCommand { get; }

    public Boolean? DialogResult {
        get => dialogResult;
        set {
            dialogResult = value;
            OnPropertyChanged();
        }
    }
}