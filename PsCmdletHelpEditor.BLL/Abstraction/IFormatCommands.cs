using System.Windows.Input;

namespace PsCmdletHelpEditor.BLL.Abstraction {
    public interface IFormatCommands {
        ICommand SetBoldCommand { get; }
        ICommand SetItalicCommand { get; }
        ICommand SetUnderlineCommand { get; }
        ICommand SetStrikeCommand { get; }
    }
}