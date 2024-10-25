using System;
using System.Windows.Input;
using CmdletHelpEditor.API.Models;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.ViewModels;
public class RelatedLinkVM : ViewModelBase {
    Boolean linkTextBoxEnabled;
    CmdletObject cmdlet;
    PsCommandRelatedLinkVM currentRelink;


    public RelatedLinkVM() {
        NewLinkCommand = new RelayCommand(newLink, canNewLink);
        RemoveLinkCommand = new RelayCommand(removeLink, canRemoveLink);
        UpLinkCommand = new RelayCommand(upLink, canUpLink);
        DownLinkCommand = new RelayCommand(downLink, canDownLink);
    }

    public ICommand NewLinkCommand { get; }
    public ICommand RemoveLinkCommand { get; }
    public ICommand UpLinkCommand { get; }
    public ICommand DownLinkCommand { get; }
    public Boolean LinkTextBoxEnabled {
        get => linkTextBoxEnabled;
        set {
            linkTextBoxEnabled = value;
            OnPropertyChanged();
        }
    }
    public PsCommandRelatedLinkVM CurrentRelink {
        get => currentRelink;
        set {
            currentRelink = value;
            LinkTextBoxEnabled = currentRelink != null;
            OnPropertyChanged();
        }
    }

    void newLink(Object obj) {
        var link = new PsCommandRelatedLinkVM { LinkText = "<Link>" };
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
        PsCommandRelatedLinkVM temp = cmdlet.RelatedLinks[old - 1];
        cmdlet.RelatedLinks[old - 1] = CurrentRelink;
        cmdlet.RelatedLinks[old] = temp;
        CurrentRelink = cmdlet.RelatedLinks[old - 1];
    }
    Boolean canUpLink(Object obj) {
        return canRemoveLink(null) && cmdlet.RelatedLinks.IndexOf(CurrentRelink) > 0;
    }
    void downLink(Object obj) {
        Int32 old = cmdlet.RelatedLinks.IndexOf(CurrentRelink);
        PsCommandRelatedLinkVM temp = cmdlet.RelatedLinks[old + 1];
        cmdlet.RelatedLinks[old + 1] = CurrentRelink;
        cmdlet.RelatedLinks[old] = temp;
        CurrentRelink = cmdlet.RelatedLinks[old + 1];
    }
    Boolean canDownLink(Object obj) {
        if (!canNewLink(null)) { return false; }
        Int32 count = cmdlet.RelatedLinks.Count - 1;
        return canRemoveLink(null) && cmdlet.RelatedLinks.IndexOf(CurrentRelink) < count;
    }

    public void SetCmdlet(CmdletObject newCmdlet) {
        cmdlet = newCmdlet;
        CurrentRelink = null;
    }
}
