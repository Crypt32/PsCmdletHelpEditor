using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using PsCmdletHelpEditor.XmlRpc;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace CmdletHelpEditor.API.ViewModels;
class ModulePropertiesVM : AsyncViewModel, IModulePublishPropertiesVM {
    Boolean useSupports, useProvider, urlEditable, provSelected, userEditable, blogsLoaded, blogSelected;
    ProviderInformation providerInfo;

    public ModulePropertiesVM() {
        ConnectProviderCommand = new AsyncCommand(connect);
    }

    public IAsyncCommand ConnectProviderCommand { get; set; }
    public ICommand FetchPostsCommand { get; set; }

    public ObservableCollection<ProviderInformation> Providers { get; } = [];
    public ObservableCollection<XmlRpcBlogInfo> WebSites { get; } = [];

    public Boolean UseSupports {
        get => useSupports;
        set {
            useSupports = value;
            OnPropertyChanged();
        }
    }
    public Boolean UseProvider {
        get => useProvider;
        set {
            useProvider = value;
            OnPropertyChanged();
        }
    }
    public Boolean ProvSelected {
        get => provSelected;
        set {
            provSelected = value;
            OnPropertyChanged();
        }
    }
    public Boolean UrlEditable {
        get => urlEditable;
        set {
            urlEditable = value;
            OnPropertyChanged();
        }
    }
    public Boolean UserEditable {
        get => userEditable;
        set {
            userEditable = value;
            OnPropertyChanged();
        }
    }
    public Boolean BlogsLoaded {
        get => blogsLoaded;
        set {
            blogsLoaded = value;
            OnPropertyChanged();
        }
    }
    public Boolean BlogSelected {
        get => blogSelected;
        set {
            blogSelected = value;
            OnPropertyChanged();
        }
    }

    Task connect(Object o, CancellationToken cancellationToken) {
        if (o is not IHasPassword pwd) {
            return Task.CompletedTask;
        }
        return Task.CompletedTask;
    }
    Boolean canConnect(Object o) {
        return false;
    }
}
