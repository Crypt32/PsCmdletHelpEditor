using System;
using PsCmdletHelpEditor.BLL.Models;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    public class EditorVM : ViewModelBase {
        Int32 paramIndex = -1;
        CmdletObject currentCmdlet;

        public EditorVM(TabItem parent) {
            ParamContext = new ParamVM();
            RelatedLinkContext = new RelatedLinkVM();
            ExampleContext = new ExampleVM();
            OutputContext = new OutputVM(parent);
        }

        public ParamVM ParamContext { get; set; }
        public RelatedLinkVM RelatedLinkContext { get; set; }
        public ExampleVM ExampleContext { get; set; }
        public OutputVM OutputContext { get; private set; }
        public CmdletObject CurrentCmdlet {
            get => currentCmdlet;
            set {
                currentCmdlet = value;
                ParamIndex = -1;
                OnPropertyChanged(nameof(CurrentCmdlet));
                ExampleContext.SetCmdlet(currentCmdlet);
                RelatedLinkContext.SetCmdlet(currentCmdlet);
            }
        }
        public Int32 ParamIndex {
            get => paramIndex;
            set {
                paramIndex = value;
                OnPropertyChanged(nameof(ParamIndex));
            }
        }
    }
}
