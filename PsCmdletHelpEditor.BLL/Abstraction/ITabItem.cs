using System;

namespace PsCmdletHelpEditor.BLL.Abstraction {
    public interface ITabItem {
        String Header { get; set; }
        ITabItemContent TabContent { get; set; }
        Boolean RequestClose();
        void Save();
    }
}