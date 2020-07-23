using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PsCmdletHelpEditor.BLL.Models;
using PsCmdletHelpEditor.BLL.Tools;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    public class ModulePropertiesVM : ClosableDialogViewModel {
        PsModuleObject m_module;
        ProviderInformation selectedProv;
        BlogInfo selectedBlog;
        Boolean useSupports, useProvider, urlEditable, provSelected, userEditable, blogsLoaded, blogSelected;

        public ModulePropertiesVM() {
            SaveCommand = new RelayCommand(save);
            // TODO
            foreach (ProviderInformation prov in Utils.EnumProviders()) {
                Providers.Add(prov);
            }
        }

        public IAsyncCommand ConnectProviderCommand { get; set; }
        public IAsyncCommand FetchPostsCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public ObservableCollection<ProviderInformation> Providers { get; }
            = new ObservableCollection<ProviderInformation>();
        public ObservableCollection<BlogInfo> WebSites { get; }
            = new ObservableCollection<BlogInfo>();

        public ProviderInformation SelectedProv {
            get => selectedProv;
            set {
                selectedProv = value;
                if (selectedProv != null) {
                    UrlEditable = true;
                    UserEditable = true;
                }
                OnPropertyChanged(nameof(SelectedProv));
            }
        }
        public BlogInfo SelectedBlog {
            get => selectedBlog;
            set {
                selectedBlog = value;
                OnPropertyChanged(nameof(SelectedBlog));
            }
        }
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

        void save(Object o) {
            m_module.Provider = SelectedProv;

            DialogResult = true;
        }

        public void SetModule(PsModuleObject module) {
            m_module = module;
            SelectedProv = module.Provider;
            SelectedBlog = module.Provider?.Blog;
            UseSupports = module.UseSupports;
            UseProvider = module.UseOnlineProvider;
            ProvSelected = SelectedProv != null;
            UrlEditable = SelectedProv != null;
        }
    }
}
