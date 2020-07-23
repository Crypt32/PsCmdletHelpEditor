using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace CmdletHelpEditor.API.ViewModels {
    class ModulePropertiesVM : AsyncViewModel, IModulePublishPropertiesVM {
        readonly IDataSource _dataSource;
        Boolean useSupports, useProvider, urlEditable, provSelected, userEditable, blogsLoaded, blogSelected;
        ProviderInformation providerInfo;

        public ModulePropertiesVM(IDataSource dataSource) {
            _dataSource = dataSource;
            ConnectProviderCommand = new AsyncCommand(connect);
        }

        public IAsyncCommand ConnectProviderCommand { get; set; }
        public ICommand FetchPostsCommand { get; set; }

        public ObservableCollection<ProviderInformation> Providers { get; }
            = new ObservableCollection<ProviderInformation>();
        public ObservableCollection<BlogInfo> WebSites { get; }
            = new ObservableCollection<BlogInfo>();

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

        Task connect(Object o, CancellationToken cancellationToken) {
            if (!(o is IHasPassword pwd)) {
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }
        Boolean canConnect(Object o) {
            return false;
        }
    }
}
