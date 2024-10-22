using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.API.Tools;
using CmdletHelpEditor.API.ViewModels;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.XmlRpc;
using PsCmdletHelpEditor.XmlRpc.WordPress;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unity;

namespace CmdletHelpEditor.Views.Dialogs;

/// <summary>
/// Interaction logic for ModuleProperties.xaml
/// </summary>
public partial class ModuleProperties : INotifyPropertyChanged, IHasPassword {
    readonly MainWindowVM _mwvm;
    readonly IMsgBox _msgbox;

    Boolean useSupports, useProvider, urlEditable, provSelected, userEditable, blogsLoaded, blogSelected;
    ProviderInformation providerInfo;
    WpXmlRpcClient blogger;

    public ModuleProperties(MainWindowVM context) {
        _mwvm = context;
        _msgbox = App.Container.Resolve<IMsgBox>();
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
        get => providerInfo;
        set {
            providerInfo = value;
            OnPropertyChanged(nameof(ProviderInfo));
        }
    }

    public ICommand UseProviderCommand { get; set; }

    public ObservableCollection<ProviderInformation> Providers { get; set; }
    public ObservableCollection<BlogInfo> WebSites { get; set; }

    public Boolean UseSupports {
        get => useSupports;
        set {
            useSupports = value;
            OnPropertyChanged(nameof(UseSupports));
        }
    }
    public Boolean UseProvider {
        get => useProvider;
        set {
            useProvider = value;
            OnPropertyChanged(nameof(UseProvider));
        }
    }
    public Boolean ProvSelected {
        get => provSelected;
        set {
            provSelected = value;
            OnPropertyChanged(nameof(ProvSelected));
        }
    }
    public Boolean UrlEditable {
        get => urlEditable;
        set {
            urlEditable = value;
            OnPropertyChanged(nameof(UrlEditable));
        }
    }
    public Boolean UserEditable {
        get => userEditable;
        set {
            userEditable = value;
            OnPropertyChanged(nameof(UserEditable));
        }
    }
    public Boolean BlogsLoaded {
        get => blogsLoaded;
        set {
            blogsLoaded = value;
            OnPropertyChanged(nameof(BlogsLoaded));
        }
    }
    public Boolean BlogSelected {
        get => blogSelected;
        set {
            blogSelected = value;
            OnPropertyChanged(nameof(BlogSelected));
        }
    }
        
    void ConnectClick(Object Sender, RoutedEventArgs e) {
        SetPassword();
        blogger = Utils.InitializeBlogger(ProviderInfo);
        try {
            IEnumerable<XmlRpcBlogInfo> blogs = Task.FromResult(blogger.GetUserBlogsAsync()).Result.Result;
            if (blogs == null) { return; }
            WebSites.Clear();
            foreach (XmlRpcBlogInfo blog in blogs) {
                var blogInfo = new BlogInfo {
                                         BlogID = blog.BlogID,
                                         BlogName = blog.BlogName,
                                         URL = blog.URL
                                     };
                WebSites.Add(blogInfo);
            }
        } catch (Exception ex) {
            _msgbox.ShowError("Error", ex.Message);
        }
        BlogsLoaded = true;
    }
    async void FetchClick(Object Sender, RoutedEventArgs e) {
        if (_mwvm.SelectedTab.Module.Provider == null) { return; }
        //List<Post<String>> posts = await MetaWeblogWrapper.GetRecentPosts(blogger, providerInfo.FetchPostCount);
        List<WpPost> posts = await blogger.GetRecentPostsAsync(providerInfo.FetchPostCount); // await MetaWeblogWrapper.GetPages(blogger, providerInfo.FetchPostCount);
        foreach (CmdletObject cmdlet in _mwvm.SelectedTab.Module.Cmdlets) {
            WpPost post = posts.FirstOrDefault(x => x.Title.Equals(cmdlet.Name));
            if (post != null) {
                cmdlet.ArticleIDString = post.PostId;
                cmdlet.URL = post.Permalink;
                if (!Uri.IsWellFormedUriString(cmdlet.URL, UriKind.Absolute)) {
                    var baseUrl = new Uri(_mwvm.SelectedTab.Module.Provider.ProviderURL);
                    cmdlet.URL = $"{baseUrl.Scheme}://{baseUrl.DnsSafeHost}{cmdlet.URL}";
                }
            }
        }
    }
    void SaveClick(Object Sender, RoutedEventArgs e) {
        _mwvm.SelectedTab.Module.UseSupports = UseSupports;
        _mwvm.SelectedTab.Module.Provider = UseProvider
            ? ProviderInfo
            : null;
    }
    void CloseClick(Object Sender, RoutedEventArgs e) {
        Close();
    }

    void ProvSelectionChanged(Object Sender, SelectionChangedEventArgs e) {
        UrlEditable = ProviderInfo != null && ProviderInfo.ProviderName == "Custom";
        UserEditable = ProviderInfo != null && !String.IsNullOrEmpty(ProviderInfo.ProviderName);
    }
    void SetPassword() {
        if (ProviderInfo.SecurePassword == null) {
            ProviderInfo.Password = pwdBox.SecurePassword.EncryptPassword();
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

    void BlogSelectionChanged(Object Sender, SelectionChangedEventArgs e) {
        if (blogger == null || ProviderInfo?.Blog == null) { return; }
            
        blogger.SetBlog(ProviderInfo.Blog.BlogID);
    }
    public SecureString Password => pwdBox.SecurePassword;

    void OnPropertyChanged(String name) {
        PropertyChangedEventHandler handler = PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    public event PropertyChangedEventHandler PropertyChanged;
}