using System;
using System.Windows.Input;
using PsCmdletHelpEditor.BLL.Models;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    public class ExampleVM : ViewModelBase {
        CmdletObject cmdlet;
        Example currentExample;

        public ExampleVM() {
            NewExampleCommand = new RelayCommand(newExample, canNewExample);
            RemoveExampleCommand = new RelayCommand(removeExample, canRemoveExample);
            UpExampleCommand = new RelayCommand(upExample, canUpExample);
            DownExampleCommad = new RelayCommand(downExample, canDownExample);
        }

        public ICommand NewExampleCommand { get; set; }
        public ICommand RemoveExampleCommand { get; set; }
        public ICommand UpExampleCommand { get; set; }
        public ICommand DownExampleCommad { get; set; }
        public Boolean ExampleTextBoxEnabled { get; set; }
        public Example CurrentExample {
            get => currentExample;
            set {
                currentExample = value;
                ExampleTextBoxEnabled = currentExample != null;
                OnPropertyChanged("CurrentExample");
                OnPropertyChanged("ExampleTextBoxEnabled");
            }
        }

        void newExample(Object obj) {
            var example = new Example();
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
            Int32 old = cmdlet.Examples.IndexOf(CurrentExample);
            Example temp = cmdlet.Examples[old - 1];
            cmdlet.Examples[old - 1] = CurrentExample;
            cmdlet.Examples[old] = temp;
            CurrentExample = cmdlet.Examples[old - 1];
        }
        Boolean canUpExample(Object obj) {
            return canRemoveExample(null) && cmdlet.Examples.IndexOf(CurrentExample) > 0;
        }
        void downExample(Object obj) {
            Int32 old = cmdlet.Examples.IndexOf(CurrentExample);
            Example temp = cmdlet.Examples[old + 1];
            cmdlet.Examples[old + 1] = CurrentExample;
            cmdlet.Examples[old] = temp;
            CurrentExample = cmdlet.Examples[old + 1];
        }
        Boolean canDownExample(Object obj) {
            if (!canNewExample(null)) { return false; }
            Int32 count = cmdlet.RelatedLinks.Count - 1;
            return canRemoveExample(null) && cmdlet.Examples.IndexOf(CurrentExample) < count;
        }

        public void SetCmdlet(CmdletObject newCmdlet) {
            cmdlet = newCmdlet;
            CurrentExample = null;
        }
    }
}
