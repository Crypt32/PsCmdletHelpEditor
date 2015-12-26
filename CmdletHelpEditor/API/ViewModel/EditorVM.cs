using CmdletHelpEditor.API.BaseClasses;
using System;
using System.ComponentModel;
using CmdletHelpEditor.Controls;

namespace CmdletHelpEditor.API.ViewModel {
	public class EditorVM : INotifyPropertyChanged {
		Int32 paramIndex = -1;
		Boolean editorEnabled;
		CmdletObject currentCmdlet;

		public EditorVM(ClosableTabItem parent) {
			ParamContext = new ParamVM();
			RelatedLinkContext = new RelatedLinkVM(this);
			ExampleContext = new ExampleVM(this);
			OutputContext = new OutputVM(parent);
		}
		public ParamVM ParamContext { get; set; }
		public RelatedLinkVM RelatedLinkContext { get; set; }
		public ExampleVM ExampleContext { get; set; }
		public OutputVM OutputContext { get; private set; }

		public Boolean GeneralTextBoxEnabled {
			get { return editorEnabled; }
			set {
				editorEnabled = value;
				OnPropertyChanged("GeneralTextBoxEnabled");
			}
		}
		public CmdletObject CurrentCmdlet {
			get { return currentCmdlet; }
			set {
				currentCmdlet = value;
				GeneralTextBoxEnabled = value != null;
				ParamIndex = -1;
				OnPropertyChanged("CurrentCmdlet");
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
