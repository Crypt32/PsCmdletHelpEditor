using CmdletHelpEditor.API.BaseClasses;
using System;
using System.ComponentModel;

namespace CmdletHelpEditor.API.ViewModel {
	public class ParamVM : INotifyPropertyChanged {
		ParameterDescription paramDescription;
		Boolean paramTextEnabled;

		public ParameterDescription CurrentParam {
			get { return paramDescription; }
			set {
				paramDescription = value;
				OnPropertyChanged("CurrentParam");
			}
		}
		public Boolean ParamTextBoxEnabled {
			get { return paramTextEnabled; }
			set {
				paramTextEnabled = value;
				OnPropertyChanged("ParamTextBoxEnabled");
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
