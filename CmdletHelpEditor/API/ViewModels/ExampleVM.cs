using System;
using System.Windows.Input;
using CmdletHelpEditor.API.Models;
using PsCmdletHelpEditor.Core.Utils;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.ViewModels;
public class ExampleVM : ViewModelBase {
    Boolean exampleTextBoxEnabled;
    CmdletObject cmdlet;
    PsCommandExampleVM currentExample;

    public ExampleVM() {
        NewExampleCommand = new RelayCommand(newExample, canNewExample);
        RemoveExampleCommand = new RelayCommand(removeExample, canRemoveExample);
        UpExampleCommand = new RelayCommand(upExample, canUpExample);
        DownExampleCommand = new RelayCommand(downExample, canDownExample);
    }

    public ICommand NewExampleCommand { get; }
    public ICommand RemoveExampleCommand { get; }
    public ICommand UpExampleCommand { get; }
    public ICommand DownExampleCommand { get; }
    public Boolean ExampleTextBoxEnabled {
        get => exampleTextBoxEnabled;
        set {
            exampleTextBoxEnabled = value;
            OnPropertyChanged();
        }
    }
    public PsCommandExampleVM CurrentExample {
        get => currentExample;
        set {
            currentExample = value;
            ExampleTextBoxEnabled = currentExample != null;
            OnPropertyChanged();
        }
    }

    void newExample(Object obj) {
        var example = new PsCommandExampleVM {
            Name = $"Example {cmdlet.Examples.Count + 1}"
        };
        cmdlet.Examples.Add(example);
        CurrentExample = example;
    }
    Boolean canNewExample(Object obj) {
        return cmdlet != null;
    }
    void removeExample(Object obj) {
        Int32 index = cmdlet.Examples.IndexOf(CurrentExample);
        cmdlet.Examples.Remove(CurrentExample);
        if (index > 0) {
            CurrentExample = cmdlet.Examples[index - 1];
        }
    }
    Boolean canRemoveExample(Object obj) {
        return CurrentExample != null;
    }
    void upExample(Object obj) {
        CurrentExample = cmdlet.Examples.MoveUp(CurrentExample);
    }
    Boolean canUpExample(Object obj) {
        return canRemoveExample(null) && cmdlet.Examples.IndexOf(CurrentExample) > 0;
    }
    void downExample(Object obj) {
        CurrentExample = cmdlet.Examples.MoveDown(CurrentExample);
    }
    Boolean canDownExample(Object obj) {
        if (!canNewExample(null)) {
            return false;
        }

        Int32 count = cmdlet.Examples.Count - 1;
        return canRemoveExample(null) && cmdlet.Examples.IndexOf(CurrentExample) < count;
    }

    public void SetCmdlet(CmdletObject newCmdlet) {
        cmdlet = newCmdlet;
        CurrentExample = null;
    }
}
