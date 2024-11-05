using System;
using System.Security;
using System.Xml.Serialization;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Models.Xml;
using PsCmdletHelpEditor.Core.Utils;
using PsCmdletHelpEditor.XmlRpc;
using SysadminsLV.WPF.OfficeTheme.Toolkit;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.Models;

public class ProviderInformation : ViewModelBase, IXmlRpcProviderInformation {
    String provName, url, userName, password;
    Int32 postCount = 50;
    XmlRpcBlogInfo blog;

    public String ProviderName {
        get => provName;
        set {
            provName = value;
            OnPropertyChanged();
        }
    }
    public String ProviderURL {
        get => url;
        set {
            url = value;
            OnPropertyChanged();
        }
    }
    public XmlRpcBlogInfo Blog {
        get => blog;
        set {
            blog = value;
            OnPropertyChanged();
        }
    }
    public String UserName {
        get => userName;
        set {
            userName = value;
            OnPropertyChanged();
        }
    }
    public String Password {
        get => password;
        set {
            password = value;
            try {
                SecurePassword = password.DecryptPassword();
            } catch (Exception e) {
                MsgBox.Show("Error", e.Message);
                SecurePassword = null;
            }
            OnPropertyChanged();
        }
    }
    [XmlIgnore]
    public SecureString SecurePassword { get; set; }
    public Int32 FetchPostCount {
        get => postCount;
        set {
            postCount = value;
            OnPropertyChanged();
        }
    }

    public WpXmlRpcClient InitializeBlogger() {
        if (
            String.IsNullOrEmpty(ProviderURL) ||
            String.IsNullOrEmpty(UserName) ||
            SecurePassword == null
        ) { return null; }
        var xProvInfo = new XmlRpcProviderInfo(ProviderURL, UserName, SecurePassword);
        var blogger = new WpXmlRpcClient(xProvInfo);
        xProvInfo.ProviderID = Blog?.BlogID;
        return blogger;
    }
    public XmlRpcProviderInformation ToXmlObject() {
        return new XmlRpcProviderInformation {
            ProviderName = ProviderName,
            ProviderURL = ProviderURL,
            Blog = Blog,
            UserName = UserName,
            Password = Password,
            FetchPostCount = FetchPostCount
        };
    }

    public static ProviderInformation? FromProviderInfo(IXmlRpcProviderInformation providerInfo) {
        if (providerInfo == null) {
            return null;
        }
        return new ProviderInformation {
            ProviderName = providerInfo.ProviderName,
            ProviderURL = providerInfo.ProviderURL,
            Blog = providerInfo.Blog,
            UserName = providerInfo.UserName,
            Password = providerInfo.Password,
            FetchPostCount = providerInfo.FetchPostCount
        };
    }
}