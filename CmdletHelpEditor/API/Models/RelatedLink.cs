using System;
using System.ComponentModel;

namespace CmdletHelpEditor.API.Models {
    public class RelatedLink : INotifyPropertyChanged {
        readonly Int32 _uid;
        String linkText, linkUrl;

        public RelatedLink() {
            _uid = Guid.NewGuid().GetHashCode();
        }

        public String LinkText {
            get => linkText ?? String.Empty;
            set {
                if (linkText != value) {
                    linkText = value;
                    OnPropertyChanged(nameof(LinkText));
                }
            }
        }
        public String LinkUrl {
            get => linkUrl ?? String.Empty;
            set {
                if (linkUrl != value) {
                    linkUrl = value;
                    OnPropertyChanged(nameof(LinkUrl));
                }
            }
        }

        public override Boolean Equals(Object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RelatedLink) obj);
        }

        protected Boolean Equals(RelatedLink other) {
            return _uid == other._uid;
        }

        public override Int32 GetHashCode() {
            unchecked {
                return _uid.GetHashCode() * 397;
            }
        }

        void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}