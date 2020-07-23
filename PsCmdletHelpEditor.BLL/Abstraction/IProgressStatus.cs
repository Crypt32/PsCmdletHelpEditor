using System;

namespace PsCmdletHelpEditor.BLL.Abstraction {
    public interface IProgressStatus {
        String StatusText { get; set; }
        Double Progress { get; set; }
    }
}