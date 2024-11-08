using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Models.Xml;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.Models;
public class CmdletObject : ViewModelBase {
    String extraHeader, extraFooter, url, articleID;
    Boolean publish;
    SupportInfo supportInfo;
    PsCommandGeneralDescriptionVM generalHelp;
    BindingList<PsCommandParameterVM> parameters;
    ObservableCollection<PsCommandExampleVM> exampleList;
    ObservableCollection<PsCommandRelatedLinkVM> linkList;

    public CmdletObject() {
        initialize();
        Examples = [];
        RelatedLinks = [];
    }

    public String Name { get; set; }
    public String Verb { get; set; }
    public String Noun { get; set; }
    public List<String> Syntax { get; set; } = [];
    public PsCommandGeneralDescriptionVM GeneralHelp {
        get => generalHelp;
        set {
            if (generalHelp != null) {
                generalHelp.PropertyChanged -= childOnPropertyChanged;
            }
            generalHelp = value;
            if (generalHelp != null) {
                generalHelp.PropertyChanged += childOnPropertyChanged;
            }
        }
    }
    public List<CommandParameterSetInfo> ParameterSets { get; set; }
    public List<CommandParameterSetInfo2> ParamSets { get; set; }
    public BindingList<PsCommandParameterVM> Parameters {
        get => parameters;
        set {
            if (parameters != null) {
                parameters.ListChanged -= OnBindingListChanged;
            }
            parameters = value;
            if (parameters != null) {
                parameters.RaiseListChangedEvents = true;
                parameters.ListChanged += OnBindingListChanged;
            }
        }
    }
    public ObservableCollection<PsCommandExampleVM> Examples {
        get => exampleList;
        set {
            if (exampleList != null) {
                exampleList.CollectionChanged -= childOnCollectionChanged;
            }
            exampleList = value ?? [];
            exampleList.CollectionChanged += childOnCollectionChanged;
        }
    }
    public ObservableCollection<PsCommandRelatedLinkVM> RelatedLinks {
        get => linkList;
        set {
            if (linkList != null) {
                linkList.CollectionChanged -= childOnCollectionChanged;
            }
            linkList = value ?? [];
            linkList.CollectionChanged += childOnCollectionChanged;
        }
    }
    public SupportInfo SupportInformation {
        get => supportInfo;
        set {
            if (supportInfo != null) {
                supportInfo.PropertyChanged -= childOnPropertyChanged;
            }
            supportInfo = value;
            if (supportInfo != null) {
                supportInfo.PropertyChanged += childOnPropertyChanged;
            }
        }
    }
    // advanced
    public String ExtraHeader {
        get => extraHeader;
        set {
            extraHeader = value;
            OnPropertyChanged();
        }
    }
    public String ExtraFooter {
        get => extraFooter;
        set {
            extraFooter = value;
            OnPropertyChanged();
        }
    }
    // rest
    public Boolean Publish {
        get => publish;
        set {
            publish = value;
            OnPropertyChanged();
        }
    }
    public String URL {
        get => url;
        set {
            url = value;
            OnPropertyChanged();
        }
    }
    public String ArticleIDString {
        get => articleID;
        set {
            articleID = value;
            OnPropertyChanged();
        }
    }
    void initialize() {
        ParamSets = [];
        Parameters = [];
        Examples = [];
        RelatedLinks = [];
    }

    #region nested object event handlers
    void OnBindingListChanged(Object Sender, ListChangedEventArgs ChangedEventArgs) {
        OnPropertyChanged("nested2");
    }
    void childOnCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
        switch (e.Action) {
            case NotifyCollectionChangedAction.Add:
                ((INotifyPropertyChanged)e.NewItems[0]).PropertyChanged += childOnPropertyChanged;
                break;
            case NotifyCollectionChangedAction.Remove:
                ((INotifyPropertyChanged)e.OldItems[0]).PropertyChanged -= childOnPropertyChanged;
                break;
            case NotifyCollectionChangedAction.Reset:
                if (e.OldItems == null) { break; }
                foreach (INotifyPropertyChanged itemToRemove in e.OldItems) {
                    itemToRemove.PropertyChanged -= childOnPropertyChanged;
                }
                break;
        }
    }
    void childOnPropertyChanged(Object sender, PropertyChangedEventArgs e) {
        OnPropertyChanged("nested");
    }
    #endregion

    public void UpdateParamSets() {
        if (ParameterSets == null) { return; }
        ParamSets.Clear();
        foreach (CommandParameterSetInfo paramInfo in ParameterSets) {
            var info = new CommandParameterSetInfo2 { Name = paramInfo.Name };
            foreach (CommandParameterInfo param in paramInfo.Parameters) {
                info.Parameters.Add(param.Name);
            }
            ParamSets.Add(info);
        }
    }

    public void CopyFromCmdlet(CmdletObject sourceCmdlet) {
        var processedParameters = new HashSet<String>(StringComparer.OrdinalIgnoreCase);
        // process saved parameters
        for (Int32 index = 0; index < Parameters.Count; index++) {
            Int32 sourceIndex = sourceCmdlet.Parameters.IndexOf(Parameters[index]);
            if (sourceIndex >= 0) {
                // copy user input to source cmdlet
                sourceCmdlet.Parameters[sourceIndex].Description = Parameters[index].Description;
                sourceCmdlet.Parameters[sourceIndex].Globbing = Parameters[index].Globbing;
                sourceCmdlet.Parameters[sourceIndex].DefaultValue = Parameters[index].DefaultValue;
                // replace parameter from source to destination cmdlet
                Parameters[index] = sourceCmdlet.Parameters[sourceIndex];
                processedParameters.Add(Parameters[index].Name);
            } else {
                // saved cmdlet contains orphaned parameter
                Parameters[index].Status = ItemStatus.Missing;
            }
        }
        // process active non-processed parameters. They are new parameters
        foreach (PsCommandParameterVM param in sourceCmdlet.Parameters.Where(param => !processedParameters.Contains(param.Name))) {
           Parameters.Add(param);
        }
    }
    public XmlPsCommand ToXmlObject() {
        return new XmlPsCommand {
            Name = Name,
            Verb = Verb,
            Noun = Noun,
            Syntax = Syntax,
            ExtraHeader = ExtraHeader,
            ExtraFooter = ExtraFooter,
            GeneralHelp = GeneralHelp.ToXmlObject(),
            ParamSets = ParamSets.Select(x => x.ToXmlObject()).ToList(),
            Parameters = Parameters
                .Where(x => x.Status != ItemStatus.Missing)
                .Select(x => x.ToXmlObject())
                .ToList(),
            Examples = Examples.Select(x => x.ToXmlObject()).ToList(),
            RelatedLinks = RelatedLinks.Select(x => x.ToXmlObject()).ToList(),
            SupportInformation = SupportInformation?.ToXmlObject(),
            Publish = Publish,
            URL = URL,
            ArticleIDString = ArticleIDString
        };
    }

    public static CmdletObject FromCommandInfo(IPsCommandInfo commandInfo) {
        var retValue = new CmdletObject();
        retValue.initialize();
        retValue.Name = commandInfo.Name;
        retValue.Verb = commandInfo.Verb;
        retValue.Noun = commandInfo.Verb;
        retValue.ExtraHeader = commandInfo.ExtraHeader;
        retValue.ExtraFooter = commandInfo.ExtraFooter;
        retValue.Syntax.AddRange(commandInfo.GetSyntax());
        retValue.GeneralHelp = new PsCommandGeneralDescriptionVM { Status = ItemStatus.New };
        retValue.GeneralHelp.ImportFromCommandInfo(commandInfo.GetDescription());
        retValue.Publish = commandInfo.Publish;
        retValue.URL = commandInfo.URL;
        retValue.ArticleIDString = commandInfo.ArticleIDString;
        retValue.SupportInformation = new SupportInfo();
        IPsCommandSupportInfo? commandSupportInfo = commandInfo.GetSupportInfo();
        if (commandSupportInfo is not null) {
            retValue.SupportInformation.RequiresAD = commandSupportInfo.RequiresAD;
            retValue.SupportInformation.RequiresRSAT = commandSupportInfo.RequiresRSAT;
            retValue.SupportInformation.SetPsVersion(commandSupportInfo.PsVersion);
            retValue.SupportInformation.SetWinOsVersion(commandSupportInfo.WinOsVersion);
        }
        if (commandInfo.IsOrphaned) {
            retValue.GeneralHelp.Status = ItemStatus.Missing;
        }
        foreach (IPsCommandParameter param in commandInfo.GetParameters()) {
            retValue.Parameters.Add(PsCommandParameterVM.ImportFromCommandInfo(param));
        }
        foreach (IPsCommandParameterSetInfo paramSet in commandInfo.GetParameterSets()) {
            retValue.ParamSets.Add(CommandParameterSetInfo2.FromCommandInfo(paramSet));
        }
        foreach (IPsCommandExample example in commandInfo.GetExamples()) {
            retValue.Examples.Add(PsCommandExampleVM.FromCommandInfo(example));
        }

        foreach (IPsCommandRelatedLink relatedLink in commandInfo.GetRelatedLinks()) {
            retValue.RelatedLinks.Add(PsCommandRelatedLinkVM.FromCommandInfo(relatedLink));
        }

        return retValue;
    }

    #region ToString

    public override String ToString() {
        return Name;
    }

    #endregion

    #region Equals

    public override Boolean Equals(Object obj) {
        return !ReferenceEquals(null, obj) && (ReferenceEquals(this, obj) ||
                                               obj.GetType() == GetType() && Equals((CmdletObject)obj));
    }
    protected Boolean Equals(CmdletObject other) {
        return String.Equals(Name, other.Name);
    }
    public override Int32 GetHashCode() {
        return Name != null ? Name.GetHashCode() : 0;
    }

    #endregion
}