using System;
using System.Security;
using System.Xml.Serialization;
using PsCmdletHelpEditor.BLL.Tools;
using SysadminsLV.WPF.OfficeTheme.Toolkit;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace PsCmdletHelpEditor.BLL.Models {
    public class ProviderInformation : ViewModelBase {
        String provName, url, userName, password;
        Int32 postCount = 50;
        BlogInfo blog;

        public String ProviderName {
            get => provName;
            set {
                provName = value;
                OnPropertyChanged(nameof(ProviderName));
            }
        }
        public String ProviderURL {
            get => url;
            set {
                url = value;
                OnPropertyChanged(nameof(ProviderURL));
            }
        }
        public BlogInfo Blog {
            get => blog;
            set {
                blog = value;
                OnPropertyChanged(nameof(Blog));
            }
        }
        public String UserName {
            get => userName;
            set {
                userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }
        public String Password {
            get => password;
            set {
                password = value;
                try {
                    SecurePassword = Crypt.DecryptPassword(password);
                } catch (Exception e) {
                    MsgBox.Show("Error", e.Message);
                    SecurePassword = null;
                }
                OnPropertyChanged(nameof(Password));
            }
        }
        [XmlIgnore]
        public SecureString SecurePassword { get; set; }
        public Int32 FetchPostCount {
            get => postCount;
            set {
                postCount = value;
                OnPropertyChanged(nameof(FetchPostCount));
            }
        }
    }
}
