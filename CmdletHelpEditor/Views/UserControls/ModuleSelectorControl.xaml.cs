using System;
using System.Windows.Input;
using CmdletHelpEditor.API.ViewModels;

namespace CmdletHelpEditor.Views.UserControls;
/// <summary>
/// Interaction logic for ModuleSelectorControl.xaml
/// </summary>
public partial class ModuleSelectorControl {
    public ModuleSelectorControl() {
        InitializeComponent();
    }

    async void lvModulesMouseDoubleClick(Object sender, MouseButtonEventArgs e) {
        //((MainWindowVM)DataContext).CommandManager.LoadCmdlets(null, false);
        await ((ModuleListDocument)DataContext).MWVM.CommandManager.LoadCommandsAsync(null, false);
    }
}
