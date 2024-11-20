using System;
using CmdletHelpEditor.API.Models;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.ViewModels;
public class EditorVM : ViewModelBase {
    readonly ModuleObject module;
    Int32 paramIndex = -1;
    CmdletObject currentCmdlet;

    public EditorVM(ModuleObject selectedModule) {
        module = selectedModule;
        ParamContext = new ParamVM();
        RelatedLinkContext = new RelatedLinkVM();
        ExampleContext = new ExampleVM();
        OutputContext = new OutputVM(selectedModule);
    }

    public ParamVM ParamContext { get; }
    public RelatedLinkVM RelatedLinkContext { get; }
    public ExampleVM ExampleContext { get; }
    public OutputVM OutputContext { get; }

    public CmdletObject CurrentCmdlet {
        get => currentCmdlet;
        set {
            module.SelectedCmdlet = currentCmdlet = value;
            ParamIndex = -1;
            OnPropertyChanged();
            ExampleContext.SetCmdlet(currentCmdlet);
            RelatedLinkContext.SetCmdlet(currentCmdlet);
        }
    }
    public Int32 ParamIndex {
        get => paramIndex;
        set {
            paramIndex = value;
            OnPropertyChanged();
        }
    }
}
