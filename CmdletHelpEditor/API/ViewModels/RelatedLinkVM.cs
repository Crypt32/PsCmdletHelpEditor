using System;
using System.ComponentModel;
using System.Windows.Input;
using CmdletHelpEditor.API.Models;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace CmdletHelpEditor.API.ViewModels {
    public class RelatedLinkVM : INotifyPropertyChanged {
        CmdletObject cmdlet;
        RelatedLink currentRelink;


        public RelatedLinkVM() {
            NewLinkCommand = new RelayCommand(newLink, canNewLink);
            RemoveLinkCommand = new RelayCommand(removeLink, canRemoveLink);
            UpLinkCommand = new RelayCommand(upLink, canUpLink);
            DownLinkCommad = new RelayCommand(downLink, canDownLink);
        }
        
        public ICommand NewLinkCommand { get; set; }
        public ICommand RemoveLinkCommand { get; set; }
        public ICommand UpLinkCommand { get; set; }
        public ICommand DownLinkCommad { get; set; }
        public Boolean LinkTextBoxEnabled { get; set; }
        public RelatedLink CurrentRelink {
            get => currentRelink;
            set {
                currentRelink = value;
                LinkTextBoxEnabled = currentRelink != null;
                OnPropertyChanged(nameof(CurrentRelink));
                OnPropertyChanged(nameof(LinkTextBoxEnabled));
            }
        }

        void newLink(Object obj) {
            var link = new RelatedLink {LinkText = "<Link>"};
            cmdlet.RelatedLinks.Add(link);
            CurrentRelink = link;
        }
        Boolean canNewLink(Object obj) {
            return cmdlet != null;
        }
        void removeLink(Object obj) {
            Int32 index = cmdlet.RelatedLinks.IndexOf(CurrentRelink);
            cmdlet.RelatedLinks.Remove(CurrentRelink);
            if (index > 0) {
                CurrentRelink = cmdlet.RelatedLinks[index - 1];
            }
        }
        Boolean canRemoveLink(Object obj) {
            return CurrentRelink != null;
        }
        void upLink(Object obj) {
            Int32 old = cmdlet.RelatedLinks.IndexOf(CurrentRelink);
            RelatedLink temp = cmdlet.RelatedLinks[old - 1];
            cmdlet.RelatedLinks[old - 1] = CurrentRelink;
            cmdlet.RelatedLinks[old] = temp;
            CurrentRelink = cmdlet.RelatedLinks[old - 1];
        }
        Boolean canUpLink(Object obj) {
            return canRemoveLink(null) && cmdlet.RelatedLinks.IndexOf(CurrentRelink) > 0;
        }
        void downLink(Object obj) {
            Int32 old = cmdlet.RelatedLinks.IndexOf(CurrentRelink);
            RelatedLink temp = cmdlet.RelatedLinks[old + 1];
            cmdlet.RelatedLinks[old + 1] = CurrentRelink;
            cmdlet.RelatedLinks[old] = temp;
            CurrentRelink = cmdlet.RelatedLinks[old + 1];
        }
        Boolean canDownLink(Object obj) {
            if (!canNewLink(null)) { return false; }
            Int32 count = cmdlet.RelatedLinks.Count - 1;
            return canRemoveLink(null) && cmdlet.RelatedLinks.IndexOf(CurrentRelink) < count;
        }

        public void SetCmdlet(CmdletObject newcmdlet) {
            cmdlet = newcmdlet;
            CurrentRelink = null;
        }

        void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
