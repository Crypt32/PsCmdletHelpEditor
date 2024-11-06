#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.API.Tools;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Services;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace CmdletHelpEditor.API.ViewModels;

public abstract class TabDocumentVM : AsyncViewModel {
    const String UNTITLED = "untitled";

    String? errorInfo, path, fileName;
    Boolean isModified, suppressModified;

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
    public String? ErrorInfo {
        get => errorInfo;
        set {
            errorInfo = value;
            OnPropertyChanged();
        }
    }

    public Boolean SupportsSave { get; set; }


    protected virtual void SaveTab(Object o) { }
    protected virtual Boolean CanSaveTab(Object o) {
        return SupportsSave;
    }
}
public sealed class BlankDocumentVM : TabDocumentVM;
public sealed class ModuleListDocument : TabDocumentVM {
    PsModuleInfo? selectedModule;

    public static ObservableCollection<PsModuleInfo> ModuleList { get; } = [];
    public PsModuleInfo? SelectedModule {
        get => selectedModule;
        set {
            selectedModule = value;
            OnPropertyChanged();
        }
    }

    public async Task ReloadModules(Boolean force, IPowerShellProcessor psProcessor) {
        StartSpinner(Strings.InfoModuleListLoading);
        ModuleList.Clear();
        IEnumerable<PsModuleInfo> modules = await psProcessor.EnumModulesAsync(force);
        foreach (PsModuleInfo moduleInfo in modules) {
            ModuleList.Add(moduleInfo);
        }
        StopSpinner();
    }
    internal MainWindowVM MWVM { get; set; }
}
public sealed class HelpProjectDocument : TabDocumentVM {
    ModuleObject? module;

    public HelpProjectDocument(ModuleObject moduleObject) {
        Module = moduleObject;
        EditorContext = new EditorVM(moduleObject);
        SupportsSave = true;
    }

    public EditorVM EditorContext { get; private set; }

    public ModuleObject Module {
        get => module;
        private set {
            if (module is not null) {
                module.PendingSave -= onModuleChanged;
            }
            module = value;
            OnPropertyChanged();
            module.PendingSave += onModuleChanged;
        }
    }
    void onModuleChanged(Object source, SavePendingEventArgs e) {
        IsModified = true;
    }
}