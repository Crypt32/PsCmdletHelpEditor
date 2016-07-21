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
			get { return currentCmdlet; }
			set {
				currentCmdlet = value;
				ParamIndex = -1;
				OnPropertyChanged("CurrentCmdlet");
                ExampleContext.SetCmdlet(currentCmdlet);
                RelatedLinkContext.SetCmdlet(currentCmdlet);
            }
		}
		public Int32 ParamIndex {
			get { return paramIndex; }
			set {
				paramIndex = value;
				OnPropertyChanged("ParamIndex");
			}
		}
		
		void OnPropertyChanged(String name) {
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
