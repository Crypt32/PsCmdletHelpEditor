using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CmdletHelpEditor.Views.Windows;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace CmdletHelpEditor.API.ViewModels;
public static class FormatCommands {
    static FormatCommands() {
        SetCommonFormatCommand = new RelayCommand(SetFormat, CanFormat);
    }

    public static ICommand SetCommonFormatCommand { get; }

    static void SetFormat(Object obj) {
        if (obj == null) { return; }
        Object[] param = (Object[]) obj;
        IInputElement fElement = FocusManager.GetFocusedElement((MainWindow)param[0]);
        if (fElement is not TextBox textBox) {
            return;
        }

        Int32 index = textBox.CaretIndex;
        textBox.SelectedText = ((Button)param[1]).Name switch {
            "Bold"      => "[b]" + textBox.SelectedText + "[/b]",
            "Italic"    => "[i]" + textBox.SelectedText + "[/i]",
            "Underline" => "[u]" + textBox.SelectedText + "[/u]",
            "Strike"    => "[s]" + textBox.SelectedText + "[/s]",
            _           => textBox.SelectedText
        };
        textBox.CaretIndex = index + 3;
    }
    static Boolean CanFormat(Object obj) {
        try {
            if (obj == null) { return false; }
            Object[] param = (Object[])obj;
            IInputElement fElement = FocusManager.GetFocusedElement((MainWindow)param[0]);
            if (fElement == null) { return false; }
            return fElement is TextBox textBox && (String)textBox.Tag == "AllowFormat";
        } catch (Exception e) {
            return false;
        }
    }
}
