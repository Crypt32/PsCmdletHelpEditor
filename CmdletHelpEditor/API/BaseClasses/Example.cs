using CmdletHelpEditor.API.Tools;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CmdletHelpEditor.API.BaseClasses {
	public class Example : INotifyPropertyChanged {
		String ename, cmd, description, output;

		public String Name {
			get { return ename ?? String.Empty; }
			set {
				ename = value;
				OnPropertyChanged("Name");
			}
		}
		public String Cmd {
			get { return cmd ?? String.Empty; }
			set {
				cmd = value;
				OnPropertyChanged("Cmd");
			}
		}
		public String Description {
			get { return description ?? String.Empty; }
			set {
				description = value;
				OnPropertyChanged("Description");
			}
		}
		public String Output {
			get { return output ?? String.Empty; }
			set {
				output = value;
				OnPropertyChanged("Output");
			}
		}

		void OnPropertyChanged(String name) {
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) {
				Utils.MarkUnsaved();
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
