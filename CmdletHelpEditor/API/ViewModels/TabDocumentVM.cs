using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using CmdletHelpEditor.API.Models;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Services;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace CmdletHelpEditor.API.ViewModels;

public abstract class TabDocumentVM : AsyncViewModel {
    const String UNTITLED = "untitled";

    String errorInfo, path, fileName;
    Boolean isModified, suppressModified, isSaved;

    protected TabDocumentVM() {
        SaveTabCommand = new RelayCommand(SaveTab, CanSaveTab);
    }

    public ICommand SaveTabCommand { get; protected set; }

    public String Header {
        get {
            String template = fileName ?? UNTITLED;
            if (IsModified) {
                template += "*";
            }

            return template;
        }
    }
    public String ToolTipText {
        get {
            if (!String.IsNullOrWhiteSpace(Path)) {
                return Path;
            }

            return UNTITLED;
        }
    }
    public String Path {
        get => path;
        set {
            path = value;
            if (!String.IsNullOrWhiteSpace(path)) {
                fileName = new FileInfo(path).Name;
            }
            OnPropertyChanged();
            OnPropertyChanged(nameof(Header));
            OnPropertyChanged(nameof(ToolTipText));
        }
    }
    public Boolean IsModified {
        get => isModified;
        set {
            isModified = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Header));
        }
    }
    public Boolean IsSaved {
        get => isSaved;
        set {
            isSaved = value;
            OnPropertyChanged();
        }
    }
    public String ErrorInfo {
        get => errorInfo;
        set {
            errorInfo = value;
            OnPropertyChanged();
        }
    }

    protected Boolean SupportsSave { get; set; }


    protected virtual void SaveTab(Object o) { }
    protected virtual Boolean CanSaveTab(Object o) {
        return SupportsSave;
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

    public async Task ReloadModules(Boolean force) {
        IsBusy = true;
        ModuleList.Clear();
        var psProcessor = new PowerShellProcessor();
        IEnumerable<PsModuleInfo> modules = await psProcessor.EnumModulesAsync(force);
        foreach (PsModuleInfo moduleInfo in modules) {
            ModuleList.Add(moduleInfo);
        }
        IsBusy = false;
    }
    internal MainWindowVM MWVM { get; set; }
}
public class HelpProjectDocument : TabDocumentVM {
    ModuleObject module;

    public EditorVM EditorContext { get; set; }

    public ModuleObject Module {
        get => module;
        set {
            module = value;
            OnPropertyChanged();
            if (module is not null) {
                EditorContext = new EditorVM(module);
            }
        }
    }
}