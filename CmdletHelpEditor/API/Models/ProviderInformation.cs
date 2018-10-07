using System;
using System.ComponentModel;
using System.Security;
using System.Xml.Serialization;
using CmdletHelpEditor.API.MetaWeblog;
using CmdletHelpEditor.API.Tools;

namespace CmdletHelpEditor.API.Models {
    public class ProviderInformation : INotifyPropertyChanged {
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
                    Utils.MsgBox("Error", e.Message);
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

        void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
