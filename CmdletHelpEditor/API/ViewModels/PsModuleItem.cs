using System;
using CmdletHelpEditor.API.Models;

namespace CmdletHelpEditor.API.ViewModels {
    public class PsModuleItem : ViewModelBase {

        public PsModuleItem() {
            Header = "Abc";
        }

        public Boolean IsSaved { get; set; }
        public String Header { get; set; }
        public ModuleObject PsModule { get; set; }
        public String ErrorText { get; set; }
    }
}