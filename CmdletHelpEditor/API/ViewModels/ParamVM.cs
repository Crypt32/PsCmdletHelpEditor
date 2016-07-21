using System;
using System.ComponentModel;
using CmdletHelpEditor.API.Models;

namespace CmdletHelpEditor.API.ViewModels {
	public class ParamVM : INotifyPropertyChanged {
		ParameterDescription paramDescription;

	    public ParameterDescription CurrentParam {
			get { return paramDescription; }
			set {
				paramDescription = value;
				OnPropertyChanged("CurrentParam");
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
