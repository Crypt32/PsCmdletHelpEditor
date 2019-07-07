using System;
using System.Windows.Input;
using PsCmdletHelpEditor.BLL.Abstraction;
using PsCmdletHelpEditor.BLL.Abstraction.Controls;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    public class FormatCommands : IFormatCommands {
        public FormatCommands() {
            SetBoldCommand = new RelayCommand(setBold, canFormat);
            SetItalicCommand = new RelayCommand(setItalic, canFormat);
            SetUnderlineCommand = new RelayCommand(setUnderline, canFormat);
            SetStrikeCommand = new RelayCommand(setStrike, canFormat);
        }

        public ICommand SetBoldCommand { get; }
        public ICommand SetItalicCommand { get; }
        public ICommand SetUnderlineCommand { get; }
        public ICommand SetStrikeCommand { get; }

        void setBold(Object o) {
            var textBox = (IFormattableTextBox)o;
            setCommonFormat("b", textBox);
        }
        void setItalic(Object o) {
            var textBox = (IFormattableTextBox)o;
            setCommonFormat("i", textBox);
        }
        void setUnderline(Object o) {
            var textBox = (IFormattableTextBox)o;
            setCommonFormat("u", textBox);
        }
        void setStrike(Object o) {
            var textBox = (IFormattableTextBox)o;
            setCommonFormat("s", textBox);
        }
        void setCommonFormat(String format, IFormattableTextBox textBox) {
            Int32 index = textBox.CaretIndex;
            textBox.SelectedText = $"[{format}]{textBox.SelectedText}[/{format}]";
            textBox.CaretIndex = index + 2 + format.Length;
        }
        Boolean canFormat(Object o) {
            if (!(o is IFormattableTextBox textBox)) {
                return false;
            }
            return textBox.AllowFormat;
        }
    }
}
