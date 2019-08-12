using System.Collections.ObjectModel;
using System.Windows.Input;
using PsCmdletHelpEditor.BLL.Abstraction;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    public class TabItemManagerVM : ViewModelBase, ITabItemManagerVM {
        ITabItem selectedTab;

        public ICommand AddTabCommand { get; set; }
        public ICommand CloseTabCommand { get; set; }

        public ObservableCollection<ITabItem> Tabs { get; }
            = new ObservableCollection<ITabItem>();
        public ITabItem SelectedTab {
            get => selectedTab;
            set {
                selectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
            }
        }
    }
}
