using System;
using System.Windows;
using SysadminsLV.WPF.OfficeTheme.Toolkit;

namespace CmdletHelpEditor.Abstract;

public interface IMsgBox {
    void ShowInfo(String header, String message);
    void ShowWarning(String header, String message);
    void ShowError(String header, String message);
}

class MsgBoxClass : IMsgBox {
    public void ShowInfo(String header, String message) {
        MsgBox.Show(header, message, MessageBoxImage.Information);
    }
    public void ShowWarning(String header, String message) {
        MsgBox.Show(header, message, MessageBoxImage.Warning);
    }
    public void ShowError(String header, String message) {
        MsgBox.Show(header, message);
    }
}