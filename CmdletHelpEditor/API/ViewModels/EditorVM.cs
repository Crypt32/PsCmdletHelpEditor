using System;
using System.ComponentModel;
using CmdletHelpEditor.API.Models;

namespace CmdletHelpEditor.API.ViewModels {
    public class EditorVM : INotifyPropertyChanged {
        Int32 paramIndex = -1;
        CmdletObject currentCmdlet;

        public EditorVM(ClosableModuleItem parent) {
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
        
        void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
