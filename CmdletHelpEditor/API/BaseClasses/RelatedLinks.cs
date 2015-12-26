using CmdletHelpEditor.API.Tools;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CmdletHelpEditor.API.BaseClasses {
	public class RelatedLink : INotifyPropertyChanged {
		String linkText, linkUrl;

		public String LinkText {
			get { return linkText ?? String.Empty; }
			set {
				linkText = value;
				OnPropertyChanged("LinkText");
			}
		}
		public String LinkUrl {
			get { return linkUrl ?? String.Empty; }
			set {
				linkUrl = value;
				OnPropertyChanged("LinkUrl");
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