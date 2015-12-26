using CmdletHelpEditor.API.MetaWeblog;
using CmdletHelpEditor.API.Tools;
using System;
using System.ComponentModel;
using System.Security;
using System.Windows;
using System.Xml.Serialization;

namespace CmdletHelpEditor.API.BaseClasses {
	public class ProviderInformation : INotifyPropertyChanged {
		String provName, url, userName, password;
		Int32 postCount = 50;
		BlogInfo blog;

		public String ProviderName {
			get { return provName; }
			set {
				provName = value;
				OnPropertyChanged("Name");
			}
		}
		public String ProviderURL {
			get { return url; }
			set {
				url = value;
				OnPropertyChanged("ProviderURL");
			}
		}
		public BlogInfo Blog {
			get { return blog; }
			set {
				blog = value;
				OnPropertyChanged("Blog");
			}
		}
		public String UserName {
			get { return userName; }
			set {
				userName = value;
				OnPropertyChanged("UserName");
			}
		}
		public String Password {
			get { return password; }
			set {
				password = value;
				try {
					SecurePassword = Crypt.DecryptPassword(password);
				} catch (Exception e) {
					Utils.MsgBox("Error", e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
					SecurePassword = null;
				}
				OnPropertyChanged("Password");
			}
		}
		[XmlIgnore]
		public SecureString SecurePassword { get; set; }
		public Int32 FetchPostCount {
			get { return postCount; }
			set {
				postCount = value;
				OnPropertyChanged("FetchPostCount");
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
