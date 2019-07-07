using System;

namespace PsCmdletHelpEditor.BLL.Abstraction.Controls {
    public interface IFormattableTextBox {
        Int32 CaretIndex { get; set; }
        String SelectedText { get; set; }
        Boolean AllowFormat { get; set; }
    }
}