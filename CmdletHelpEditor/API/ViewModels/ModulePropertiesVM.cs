using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CmdletHelpEditor.API.Models;

namespace CmdletHelpEditor.API.ViewModels {
    class ModulePropertiesVM : ViewModelBase {
        Boolean useSupports, useProvider, urlEditable, provSelected, userEditable, blogsLoaded, blogSelected;
        ProviderInformation providerInfo;

        public ICommand ConnectProviderCommand { get; set; }
        public ICommand FetchPostsCommand { get; set; }

        public ObservableCollection<ProviderInformation> Providers { get; set; }
        public ObservableCollection<BlogInfo> WebSites { get; set; }

        public Boolean UseSupports {
            get => useSupports;
            set {
                useSupports = value;
                OnPropertyChanged("UseSupports");
            }
        }
        public Boolean UseProvider {
            get => useProvider;
            set {
                useProvider = value;
                OnPropertyChanged("UseProvider");
            }
        }
        public Boolean ProvSelected {
            get => provSelected;
            set {
                provSelected = value;
                OnPropertyChanged("ProvSelected");
            }
        }
        public Boolean UrlEditable {
            get => urlEditable;
            set {
                urlEditable = value;
                OnPropertyChanged("UrlEditable");
            }
        }
        public Boolean UserEditable {
            get => userEditable;
            set {
                userEditable = value;
                OnPropertyChanged("UserEditable");
            }
        }
        public Boolean BlogsLoaded {
            get => blogsLoaded;
            set {
                blogsLoaded = value;
                OnPropertyChanged("BlogsLoaded");
            }
        }
        public Boolean BlogSelected {
            get => blogSelected;
            set {
                blogSelected = value;
                OnPropertyChanged("BlogSelected");
            }
        }
    }
}
