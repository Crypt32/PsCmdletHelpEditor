using System;
using CmdletHelpEditor.API.Abstractions;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.Models {
    public class RelatedLink : ViewModelBase, IPsRelatedLink {
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
            return !ReferenceEquals(null, obj) && (ReferenceEquals(this, obj) ||
                                                   obj.GetType() == this.GetType() && Equals((RelatedLink)obj));
        }

        protected Boolean Equals(RelatedLink other) {
            return _uid == other._uid;
        }

        public override Int32 GetHashCode() {
            unchecked {
                return _uid.GetHashCode() * 397;
            }
        }
    }
}