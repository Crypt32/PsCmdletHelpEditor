using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace PsCmdletHelpEditor.BLL.Abstraction {
    public interface ITabItemManagerVM {
        ICommand AddTabCommand { get; set; }
        ICommand CloseTabCommand { get; set; }
        ObservableCollection<ITabItem> Tabs { get; }
        ITabItem SelectedTab { get; set; }
        event PropertyChangedEventHandler PropertyChanged;
    }
}