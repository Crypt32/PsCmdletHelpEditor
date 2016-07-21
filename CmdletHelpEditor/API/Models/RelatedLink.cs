using System;
using System.ComponentModel;
using CmdletHelpEditor.API.Tools;

namespace CmdletHelpEditor.API.Models {
	public class RelatedLink : INotifyPropertyChanged {
	    readonly Int32 _uid;
		String linkText, linkUrl;

        public RelatedLink() {
            _uid = Guid.NewGuid().GetHashCode();
        }

		public String LinkText {
			get { return linkText ?? String.Empty; }
			set {
                if (linkText != value) {
                    linkText = value;
                    OnPropertyChanged("LinkText");
                }
			}
		}
		public String LinkUrl {
			get { return linkUrl ?? String.Empty; }
			set {
                if (linkUrl != value) {
                    linkUrl = value;
                    OnPropertyChanged("LinkUrl");
                }
			}
		}

        public override Boolean Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RelatedLink) obj);
        }

	    protected bool Equals(RelatedLink other) {
            return _uid == other._uid;
	    }

	    public override int GetHashCode() {
	        unchecked {
                return _uid.GetHashCode() * 397;
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