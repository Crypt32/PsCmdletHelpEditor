using PsCmdletHelpEditor.Core.Models;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CmdletHelpEditor.API.ViewModels;

public abstract class TabDocumentVM : AsyncViewModel {
    String title = "untitled2";

    protected TabDocumentVM() {
        SaveTabCommand = new RelayCommand(SaveTab, CanSaveTab);
    }

    public ICommand SaveTabCommand { get; protected set; }

    public String Header {
        get => title;
        set {
            title = value;
            OnPropertyChanged();
        }
    }

    

    protected virtual void SaveTab(Object o) { }
    protected virtual Boolean CanSaveTab(Object o) {
        return false;
    }
}
public class BlankDocumentVM : TabDocumentVM;
public class ModuleListDocument : TabDocumentVM {
    PsModuleInfo selectedModule;

    public ObservableCollection<PsModuleInfo> ModuleList { get; } = [];
    public PsModuleInfo SelectedModule {
        get => selectedModule;
        set {
            selectedModule = value;
            OnPropertyChanged();
        }
    }
}
public class HelpProjectDocument {

}