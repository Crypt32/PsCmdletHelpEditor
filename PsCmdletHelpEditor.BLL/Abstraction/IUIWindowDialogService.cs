using System;

namespace PsCmdletHelpEditor.BLL.Abstraction {
    public interface IUIWindowDialogService {
        Boolean? ShowDialog(String title, Object dataContext);
    }
}