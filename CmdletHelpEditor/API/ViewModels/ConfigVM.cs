using System;
using System.Windows;
using System.Windows.Input;
using CmdletHelpEditor.Properties;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace CmdletHelpEditor.API.ViewModels;
public class ConfigVM : DependencyObject {
    public ConfigVM() {
        SaveConfigCommand = new RelayCommand(SaveConfig);
    }

    public ICommand SaveConfigCommand { get; set; }

    static void SaveConfig(Object obj) {
        Settings.Default.Save();
    }
}
