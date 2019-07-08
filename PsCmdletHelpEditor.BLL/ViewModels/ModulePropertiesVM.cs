using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PsCmdletHelpEditor.BLL.Models;
using PsCmdletHelpEditor.XmlRpc;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    class ModulePropertiesVM : ViewModelBase {
        Boolean useSupports, useProvider, urlEditable, provSelected, userEditable, blogsLoaded, blogSelected;

        public ICommand ConnectProviderCommand { get; set; }
        public ICommand FetchPostsCommand { get; set; }

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
    }
}
