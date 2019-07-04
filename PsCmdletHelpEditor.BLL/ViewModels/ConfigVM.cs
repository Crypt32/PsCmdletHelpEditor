using System;
using System.Windows;
using System.Windows.Input;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    public class ConfigVM : DependencyObject {
        public ConfigVM() {
            SaveConfigCommand = new RelayCommand(SaveConfig);
        }

        public ICommand SaveConfigCommand { get; set; }

        static void SaveConfig(Object obj) {
            Settings.Default.Save();
        }
    }
}
