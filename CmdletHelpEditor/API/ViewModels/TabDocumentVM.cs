using PsCmdletHelpEditor.Core.Models;
using System;
using System.Collections.ObjectModel;

namespace CmdletHelpEditor.API.ViewModels;

public abstract class TabDocumentVM : AsyncViewModel {
    String title = "untitled2";

    public String Header {
        get => title;
        set {
            title = value;
            OnPropertyChanged();
        }
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