using System;
using PsCmdletHelpEditor.BLL.Abstraction;
using PsCmdletHelpEditor.Wpf.Views.Windows;

namespace PsCmdletHelpEditor.Wpf.Views {
    public class WpfUIWindowDialogService : IUIWindowDialogService {
        public Boolean? ShowDialog(String title, Object dataContext) {
            var win = new WindowDialog {
                Title = title,
                DataContext = dataContext
            };

            return win.ShowDialog();
        }
    }
}
