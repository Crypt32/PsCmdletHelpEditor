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
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.API.Tools;
using CmdletHelpEditor.API.ViewModels;
using PsCmdletHelpEditor.Core.Utils;
using PsCmdletHelpEditor.XmlRpc;
using PsCmdletHelpEditor.XmlRpc.WordPress;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using Unity;

namespace CmdletHelpEditor.Views.Dialogs;

/// <summary>
/// Interaction logic for ModuleProperties.xaml
/// </summary>
public partial class ModuleProperties : INotifyPropertyChanged, IHasPassword {
    readonly HelpProjectDocument _context;
    readonly IMsgBox _msgBox;

    Boolean useSupports, useProvider, urlEditable, provSelected, userEditable, blogsLoaded, blogSelected;
    ProviderInformation providerInfo;
    WpXmlRpcClient blogger;

    public ModuleProperties(HelpProjectDocument context) {
        _context = context;
        _msgBox = App.Container.Resolve<IMsgBox>();
        UseProviderCommand = new RelayCommand(UseProviderChanged);
        UseSupports = _context.Module.UseSupports;
        Providers = new ObservableCollection<ProviderInformation>(Utils.EnumProviders());
        InitializeComponent();
        if (_context.Module.Provider != null) {
            blogger = _context.Module.Provider.InitializeBlogger();
            Providers.Clear();
            Providers.Add(_context.Module.Provider);
            WebSites.Add(_context.Module.Provider.Blog);
            UseProvider = true;
            ProvSelected = true;
            UserEditable = true;
            ProviderInfo = _context.Module.Provider;
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

    public ObservableCollection<ProviderInformation> Providers { get; set; } = [];
    public ObservableCollection<XmlRpcBlogInfo> WebSites { get; set; } = [];

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
        blogger = ProviderInfo.InitializeBlogger();
        try {
            IEnumerable<PsCmdletHelpEditor.XmlRpc.XmlRpcBlogInfo> blogs = Task.FromResult(blogger.GetUserBlogsAsync()).Result.Result;
            if (blogs == null) { return; }
            WebSites.Clear();
            foreach (PsCmdletHelpEditor.XmlRpc.XmlRpcBlogInfo blog in blogs) {
                var blogInfo = new XmlRpcBlogInfo {
                    BlogID = blog.BlogID,
                    BlogName = blog.BlogName,
                    URL = blog.URL
                };
                WebSites.Add(blogInfo);
            }
        } catch (Exception ex) {
            _msgBox.ShowError("Error", ex.Message);
        }
        BlogsLoaded = true;
    }
    async void FetchClick(Object Sender, RoutedEventArgs e) {
        if (_context.Module.Provider is null) {
            return;
        }
        //List<Post<String>> posts = await MetaWeblogWrapper.GetRecentPosts(blogger, providerInfo.FetchPostCount);
        List<WpPost> posts = await blogger.GetRecentPostsAsync(providerInfo.FetchPostCount); // await MetaWeblogWrapper.GetPages(blogger, providerInfo.FetchPostCount);
        foreach (CmdletObject cmdlet in _context.Module.Cmdlets) {
            WpPost post = posts.FirstOrDefault(x => x.Title.Equals(cmdlet.Name));
            if (post != null) {
                cmdlet.ArticleIDString = post.PostId;
                cmdlet.URL = post.Permalink;
                if (!Uri.IsWellFormedUriString(cmdlet.URL, UriKind.Absolute)) {
                    var baseUrl = new Uri(_context.Module.Provider.ProviderURL);
                    cmdlet.URL = $"{baseUrl.Scheme}://{baseUrl.DnsSafeHost}{cmdlet.URL}";
                }
            }
        }
    }
    void SaveClick(Object Sender, RoutedEventArgs e) {
        _context.Module.UseSupports = UseSupports;
        _context.Module.Provider = UseProvider
            ? ProviderInfo
            : null;
    }
    void CloseClick(Object Sender, RoutedEventArgs e) {
        Close();
    }

    void ProvSelectionChanged(Object Sender, SelectionChangedEventArgs e) {
        UrlEditable = ProviderInfo is { ProviderName: "Custom" };
        UserEditable = ProviderInfo != null && !String.IsNullOrEmpty(ProviderInfo.ProviderName);
    }
    void SetPassword() {
        if (ProviderInfo.SecurePassword is null) {
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