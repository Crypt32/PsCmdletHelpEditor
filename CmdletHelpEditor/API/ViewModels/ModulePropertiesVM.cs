using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CmdletHelpEditor.API.MetaWeblog;
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
    }
}
