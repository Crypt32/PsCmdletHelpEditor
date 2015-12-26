using System;
using System.ComponentModel;

namespace CmdletHelpEditor.API.MetaWeblog {
	public class BlogInfo : INotifyPropertyChanged {
		String _blogId, _blogName, _url;

		public String blogid {
			get { return _blogId; }
			set {
				_blogId = value;
				OnPropertyChanged("blogid");
			}
		}
		public String blogName {
			get { return _blogName; }
			set {
				_blogName = value;
				OnPropertyChanged("blogName");
			}
		}
		public String url {
			get { return _url; }
			set {
				_url = value;
				OnPropertyChanged("url");
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
