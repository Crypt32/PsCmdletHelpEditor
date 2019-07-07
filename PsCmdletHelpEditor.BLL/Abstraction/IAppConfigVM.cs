using System;

namespace PsCmdletHelpEditor.BLL.Abstraction {
    public interface IAppConfigVM {
        Boolean LoadPsFunctions { get; set; }
        Boolean LoadPsFilter { get; set; }
        Boolean LoadPsCmdlets { get; set; }
        Boolean LoadPsScripts { get; set; }
        Boolean LoadPsWorkflows { get; set; }
        Boolean LoadPsApplications { get; set; }
        Boolean LoadPsDscConfigurations { get; set; }
        Boolean LoadExternalPsScripts { get; set; }
        Boolean SupportsWorkflow { get; set; }
        Boolean SupportsDsc { get; set; }
        Boolean ShowToolbar { get; set; }
        Boolean ShowStatusBar { get; set; }
        Boolean ToolbarLocked { get; set; }

        String GetCommandTypesString();
    }
}