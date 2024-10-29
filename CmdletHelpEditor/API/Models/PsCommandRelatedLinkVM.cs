using System;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Models.Xml;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.Models;

public class PsCommandRelatedLinkVM : ViewModelBase, IPsCommandRelatedLink {
    readonly Int32 _uid;
    String linkText, linkUrl;

    public PsCommandRelatedLinkVM() {
        _uid = Guid.NewGuid().GetHashCode();
    }

    public String LinkText {
        get => linkText ?? String.Empty;
        set {
            if (linkText != value) {
                linkText = value;
                OnPropertyChanged();
            }
        }
    }
    public String LinkUrl {
        get => linkUrl ?? String.Empty;
        set {
            if (linkUrl != value) {
                linkUrl = value;
                OnPropertyChanged();
            }
        }
    }

    public XmlPsCommandRelatedLink ToXmlObject() {
        return new XmlPsCommandRelatedLink {
            LinkText = LinkText,
            LinkUrl = LinkUrl,
        };
    }

    public static PsCommandRelatedLinkVM FromCommandInfo(IPsCommandRelatedLink relatedLink) {
        return new PsCommandRelatedLinkVM {
            LinkText = relatedLink.LinkText,
            LinkUrl = relatedLink.LinkUrl,
        };
    }

    #region Equals

    public override Boolean Equals(Object obj) {
        return !ReferenceEquals(null, obj) && (ReferenceEquals(this, obj) ||
                                               obj.GetType() == this.GetType() && Equals((PsCommandRelatedLinkVM)obj));
    }

    protected Boolean Equals(PsCommandRelatedLinkVM other) {
        return _uid == other._uid;
    }

    public override Int32 GetHashCode() {
        unchecked {
            return _uid.GetHashCode() * 397;
        }
    }

    #endregion
}