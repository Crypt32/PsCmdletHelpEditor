using CmdletHelpEditor.API.MetaWeblog;
using CmdletHelpEditor.API.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.API.ViewModels;

namespace CmdletHelpEditor.UI.Dialogs {
	/// <summary>
	/// Interaction logic for ModuleProperties.xaml
	/// </summary>
	public partial class ModuleProperties : INotifyPropertyChanged {
		Boolean useSupports, useProvider, urlEditable, provSelected, userEditable, blogsLoaded, blogSelected;
		ProviderInformation providerInfo;
		Blogger blogger;
		readonly MainWindowVM _mwvm;

		public ModuleProperties(MainWindowVM context) {
			_mwvm = context;
			UseProviderCommand = new RelayCommand(UseProviderChanged);
			UseSupports = _mwvm.SelectedTab.Module.UseSupports;
			Providers = new ObservableCollection<ProviderInformation>(Utils.EnumProviders());
			WebSites = new ObservableCollection<BlogInfo>();
			InitializeComponent();
			if (context.SelectedTab.Module.Provider != null) {
				blogger = Utils.InitializeBlogger(context.SelectedTab.Module.Provider);
				Providers.Clear();
				Providers.Add(context.SelectedTab.Module.Provider);
				WebSites.Add(context.SelectedTab.Module.Provider.Blog);
				UseProvider = true;
				ProvSelected = true;
				UserEditable = true;
				ProviderInfo = context.SelectedTab.Module.Provider;
			}
		}

		public ProviderInformation ProviderInfo {
			get { return providerInfo; }
			set {
				providerInfo = value;
				OnPropertyChanged("ProviderInfo");
			}
		}

		public ICommand UseProviderCommand { get; set; }

		public ObservableCollection<ProviderInformation> Providers { get; set; }
		public ObservableCollection<BlogInfo> WebSites { get; set; }

		public Boolean UseSupports {
			get { return useSupports; }
			set {
				useSupports = value;
				OnPropertyChanged("UseSupports");
			}
		}
		public Boolean UseProvider {
			get { return useProvider; }
			set {
				useProvider = value;
				OnPropertyChanged("UseProvider");
			}
		}
		public Boolean ProvSelected {
			get { return provSelected; }
			set {
				provSelected = value;
				OnPropertyChanged("ProvSelected");
			}
		}
		public Boolean UrlEditable {
			get { return urlEditable; }
			set {
				urlEditable = value;
				OnPropertyChanged("UrlEditable");
			}
		}
		public Boolean UserEditable {
			get { return userEditable; }
			set {
				userEditable = value;
				OnPropertyChanged("UserEditable");
			}
		}
		public Boolean BlogsLoaded {
			get { return blogsLoaded; }
			set {
				blogsLoaded = value;
				OnPropertyChanged("BlogsLoaded");
			}
		}
		public Boolean BlogSelected {
			get { return blogSelected; }
			set {
				blogSelected = value;
				OnPropertyChanged("BlogSelected");
			}
		}
		
		void ConnectClick(object Sender, RoutedEventArgs e) {
			SetPassword();
			blogger = Utils.InitializeBlogger(ProviderInfo);
			IEnumerable<BlogInfo> blogs = blogger.GetUsersBlogs();
			if (blogs == null) { return; }
			WebSites.Clear();
			foreach (BlogInfo blog in blogs) {
				WebSites.Add(blog);
			}
			BlogsLoaded = true;
		}
		async void FetchClick(object Sender, RoutedEventArgs e) {
			if (_mwvm.SelectedTab.Module.Provider == null) { return; }
			List<Post> posts = await MetaWeblogWrapper.GetRecentPosts(blogger, _mwvm.SelectedTab.Module.Cmdlets, providerInfo.FetchPostCount);
			foreach (CmdletObject cmdlet in _mwvm.SelectedTab.Module.Cmdlets) {
				Int32 index = posts.IndexOf(new Post { Title = cmdlet.Name });
				if (index >= 0) {
					cmdlet.ArticleIDString = posts[index].PostId;
					cmdlet.URL = _mwvm.SelectedTab.Module.Provider.ProviderName.ToLower() == "codeplex"
						? _mwvm.SelectedTab.Module.Provider.Blog.url + "wikipage?title=" + cmdlet.Name
						: posts[index].Permalink;
                    if (!Uri.IsWellFormedUriString(cmdlet.URL, UriKind.Absolute)) {
                        var baseUrl = new Uri(_mwvm.SelectedTab.Module.Provider.ProviderURL);
                        cmdlet.URL = String.Format("{0}://{1}{2}", baseUrl.Scheme, baseUrl.DnsSafeHost, cmdlet.URL);
                    }
                }
			}
		}
		void SaveClick(object Sender, RoutedEventArgs e) {
			_mwvm.SelectedTab.Module.UseSupports = UseSupports;
			_mwvm.SelectedTab.Module.Provider = UseProvider
				? ProviderInfo
				: null;
		}
		void CloseClick(object Sender, RoutedEventArgs e) {
			Close();
		}

		void ProvSelectionChanged(object Sender, SelectionChangedEventArgs e) {
			UrlEditable = ProviderInfo != null && ProviderInfo.ProviderName == "Custom";
			UserEditable = ProviderInfo != null && !String.IsNullOrEmpty(ProviderInfo.ProviderName);
		}
		void SetPassword() {
			if (ProviderInfo.SecurePassword == null) {
				ProviderInfo.Password = Crypt.EncryptPassword(pwdBox.SecurePassword);
				pwdBox.Clear();
			}
		}
		void UseProviderChanged(Object obj) {
			if (UseProvider) {
				ProviderInfo = new ProviderInformation();
			} else {
				ProviderInfo = null;
				BlogsLoaded = false;
			}
		}

		void OnPropertyChanged(String name) {
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;

		void BlogSelectionChanged(object Sender, SelectionChangedEventArgs e) {
			if (blogger == null || ProviderInfo == null || ProviderInfo.Blog == null) { return; }
			blogger.SetBlog(ProviderInfo.Blog.blogid);
		}
	}
}
