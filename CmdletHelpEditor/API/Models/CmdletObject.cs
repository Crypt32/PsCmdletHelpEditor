using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Xml.Serialization;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Models.Xml;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.Models;
[XmlInclude(typeof(PsCommandParameterVM))]
[XmlInclude(typeof(PsCommandExampleVM))]
[XmlInclude(typeof(PsCommandRelatedLinkVM))]
[XmlInclude(typeof(CommandParameterSetInfo2))]
public class CmdletObject : ViewModelBase {
    String extraHeader, extraFooter, url, articleID;
    Boolean publish;
    SupportInfo supportInfo;
    PsCommandGeneralDescriptionVM generalHelp;
    BindingList<PsCommandParameterVM> parameters;
    ObservableCollection<PsCommandExampleVM> exampleList;
    ObservableCollection<PsCommandRelatedLinkVM> linkList;
    readonly List<String> _excludedParams = [
        "verbose", "debug", "erroraction", "errorvariable", "outvariable", "outbuffer",
        "warningvariable", "warningaction", "pipelinevariable", "informationaction",
        "informationvariable"
    ];

    public CmdletObject() {
        initialize();
        Examples = [];
        RelatedLinks = [];
    }

    public CmdletObject(PSObject cmdlet, Boolean contentBasedHelp) : this() {
        if (cmdlet == null) { return; }
        initializeFromCmdlet(cmdlet);
        if (contentBasedHelp) {
            initializeFromHelp(cmdlet);
        }
    }
    public CmdletObject(String name) : this() {
        Name = name;
        GeneralHelp = new PsCommandGeneralDescriptionVM { Status = ItemStatus.Missing };
    }

    public String Name { get; set; }
    [XmlAttribute("verb")]
    public String Verb { get; set; }
    [XmlAttribute("noun")]
    public String Noun { get; set; }
    public List<String> Syntax { get; set; }
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
    [XmlIgnore]
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

    void initializeFromCmdlet(PSObject cmdlet) {
        Name = (String)cmdlet.Members["Name"].Value;
        Verb = (String)cmdlet.Members["Verb"].Value;
        Noun = (String)cmdlet.Members["Noun"].Value;
        GeneralHelp = new PsCommandGeneralDescriptionVM {Status = ItemStatus.New};
        Parameters = [];
        ParamSets = [];
        getParameterSets(cmdlet);
        getParameters();
        getOutputTypes(cmdlet);
        getSyntax(cmdlet);
        RelatedLinks = [];
        Examples = [];
        SupportInformation = new SupportInfo();
    }
    void initializeFromHelp(PSObject cmdlet) {
        var help = (PSObject) cmdlet.Members["syn"].Value;
        if (help.Members["Description"].Value == null) { return; }
        if (String.IsNullOrEmpty((String)help.Members["Synopsis"].Value)) { return; }
        // synopsis
        GeneralHelp.Synopsis = (String)help.Members["Synopsis"].Value;
        // description
        String description = ((PSObject[]) help.Members["Description"].Value)
            .Aggregate(String.Empty, (current, paragraph) => current + (paragraph.Members["Text"].Value + Environment.NewLine));
        GeneralHelp.Description = description.TrimEnd();
        // notes
        if (help.Properties["alertSet"] != null) {
            importNotesHelp((PSObject)help.Members["alertSet"].Value);
        }
        // input type
        if (help.Properties["inputTypes"] != null) {
            importTypes((PSObject)help.Members["inputTypes"].Value, true);
        }
        // return type
        if (help.Properties["returnValues"] != null) {
            importTypes((PSObject)help.Members["returnValues"].Value, false);
        }
        // parameters
        try {
            var param = (PSObject)help.Members["parameters"].Value;
            importParamHelp(param);
        } catch { }
        // examples
        try {
            var examples = (PSObject)help.Members["examples"].Value;
            importExamples(examples);
        } catch { }
        // related links
        try {
            var relinks = (PSObject)help.Members["relatedLinks"].Value;
            importRelinks(relinks);
        } catch { }
    }
    void importNotesHelp(PSObject note) {
        GeneralHelp.Notes = (String)((PSObject[])note.Members["alert"].Value)[0].Members["Text"].Value;
    }
    void importTypes(PSObject types, Boolean input) {
        var iType = new List<PSObject>();
        if (input) {
            if (types.Members["inputType"].Value is PSObject value) {
                iType.Add(value);
            } else {
                iType = [..(PSObject[])types.Members["inputType"].Value];
            }
        } else {
            if (types.Members["returnValue"].Value is PSObject value) {
                iType.Add(value);
            } else {
                iType = [..(PSObject[])types.Members["returnValue"].Value];
            }
        }
        
        var links = new List<String>();
        var urls = new List<String>();
        var descs = new List<String>();

        foreach (PSObject type in iType) {
            var internalType = (PSObject) type.Members["type"].Value;
            if (internalType.Properties["name"] == null) {
                links.Add(String.Empty);
            } else {
                var name = (PSObject)internalType.Members["name"].Value;
                links.Add((String)name.BaseObject);
            }
            if (internalType.Properties["uri"] == null) {
                urls.Add(String.Empty);
            } else {
                var uri = (PSObject)internalType.Members["uri"].Value;
                urls.Add((String)uri.BaseObject);
            }

            if (type.Properties["description"] == null) { continue; }
            if (type.Members["description"].Value is PSObject[] descriptionBase) {
                String description = (descriptionBase)
                    .Aggregate(String.Empty, (current, paragraph) => current + (paragraph.Members["Text"].Value + Environment.NewLine));
                descs.Add(String.IsNullOrEmpty(description)
                    ? String.Empty
                    : description.TrimEnd());
            }
        }
        if (input) {
            GeneralHelp.InputType = String.Join(";", links);
            GeneralHelp.InputUrl = String.Join(";", urls);
            GeneralHelp.InputTypeDescription = String.Join(";", descs);
        } else {
            GeneralHelp.ReturnType = String.Join(";", links);
            GeneralHelp.ReturnUrl = String.Join(";", urls);
            GeneralHelp.ReturnTypeDescription = String.Join(";", descs);
        }
    }
    void importParamHelp(PSObject helpParameters) {
        var paras = new List<PSObject>();
        if (!(helpParameters.Members["parameter"].Value is PSObject)) {
            paras = [..(PSObject[])helpParameters.Members["parameter"].Value];
        } else {
            paras.Add((PSObject)helpParameters.Members["parameter"].Value);
        }
        foreach (PSObject param in paras) {
            String name = (String)((PSObject)param.Members["name"].Value).BaseObject;
            String description = ((PSObject[]) param.Members["Description"].Value)
            .Aggregate(String.Empty, (current, paragraph) => current + (paragraph.Members["Text"].Value + Environment.NewLine));
            String defaultValue = (String)((PSObject)param.Members["defaultValue"].Value).BaseObject;
            PsCommandParameterVM currentParam = Parameters.Single(x => x.Name == name);
            currentParam.Description = description;
            currentParam.DefaultValue = defaultValue;
        }
    }
    void importExamples(PSObject example) {
        var examples = new List<PSObject>();
        if (!(example.Members["example"].Value is PSObject)) {
            examples = [..(PSObject[])example.Members["example"].Value];
        } else {
            examples.Add((PSObject)example.Members["example"].Value);
        }
        foreach (PSObject ex in examples) {
            String title = ((String)((PSObject)ex.Members["title"].Value).BaseObject).Replace("-", String.Empty).Trim();
            String code = (String)((PSObject)ex.Members["code"].Value).BaseObject;
            String description = ((PSObject[])ex.Members["remarks"].Value)
            .Aggregate(String.Empty, (current, paragraph) => current + (paragraph.Members["Text"].Value + Environment.NewLine));
            Examples.Add(new PsCommandExampleVM {
                Name = title,
                Cmd = code,
                Description = description
            });
        }
    }
    void importRelinks(PSObject relink) {
        var relinks = new List<PSObject>();
        if (!(relink.Members["navigationLink"].Value is PSObject)) {
            relinks = [..(PSObject[])relink.Members["navigationLink"].Value];
        } else {
            relinks.Add((PSObject)relink.Members["navigationLink"].Value);
        }
        foreach (PSObject link in relinks) {
            String linkText = ((String)((PSObject)link.Members["linkText"].Value).BaseObject).Replace("-", String.Empty).Trim();
            String uri = (String)((PSObject)link.Members["uri"].Value).BaseObject;
            RelatedLinks.Add(new PsCommandRelatedLinkVM {
                LinkText = linkText,
                LinkUrl = uri
            });
        }
    }
    void getParameterSets(PSObject cmdlet) {
        ParameterSets = [];
        if (cmdlet.Members["ParameterSets"].Value != null) {
            ParameterSets = [..(ReadOnlyCollection<CommandParameterSetInfo>)cmdlet.Members["ParameterSets"].Value];
            foreach (CommandParameterSetInfo paramInfo in ParameterSets) {
                var info = new CommandParameterSetInfo2 { Name = paramInfo.Name };
                foreach (CommandParameterInfo param in paramInfo.Parameters) {
                    info.Parameters.Add(param.Name);
                }
                ParamSets.Add(info);
            }
        }
    }
    void getParameters() {
        if (ParameterSets.Count == 0) { return; }
        foreach (CommandParameterSetInfo paramSet in ParameterSets) {
            if (paramSet.Parameters.Count == 0) { return; }
                foreach (CommandParameterInfo param in from param in paramSet.Parameters where !_excludedParams.Contains(param.Name.ToLower()) let para = new PsCommandParameterVM(param) where !Parameters.Contains(para) select param) {
                    Parameters.Add(new PsCommandParameterVM(param));
            }
        }
    }
    void getOutputTypes(PSObject cmdlet) {
        PSMemberInfo outputTypeMember = cmdlet.Members["OutputType"];
        if (!(outputTypeMember?.Value is IEnumerable<PSTypeName> outputTypeNames)) {
            return;
        }
        String joined = String.Join(";", outputTypeNames.Select(tn => tn.Name));
        GeneralHelp.ReturnType = joined;
    }
    void getSyntax(PSObject cmdlet) {
        Syntax = [];
        foreach (CommandParameterSetInfo paramSet in ParameterSets) {
            String syntaxItem = Convert.ToString(cmdlet.Members["name"].Value);
            foreach (CommandParameterInfo item in paramSet.Parameters) {
                if (_excludedParams.Contains(item.Name.ToLower())) { continue; }
                Boolean named = item.Position < 0;
                // fetch param type
                String paramType = String.Empty;
                CommandParameterInfo item1 = item;
                foreach (PsCommandParameterVM param in Parameters.Where(param => item1.Name == param.Name)) {
                    paramType = param.Type;
                }
                // fetch ValidateSet attribute
                String validateSet = String.Empty;
                foreach (Attribute attribute in item.Attributes) {
                    Boolean found = false;
                    validateSet = String.Empty;
                    switch (attribute.TypeId.ToString()) {
                        case "System.Management.Automation.ValidateSetAttribute":
                            validateSet += " {";
                            validateSet += String.Join(" | ", ((ValidateSetAttribute)attribute).ValidValues);
                            validateSet += "} ";
                            found = true;
                            break;
                    }
                    if (found) { break; }
                }
                if (item.IsMandatory && named) {
                    syntaxItem += " -" + item.Name + " <" + paramType + ">" + validateSet;
                } else if (item.IsMandatory) {
                    syntaxItem += " [-" + item.Name + "] <" + paramType + ">" + validateSet;
                } else if (!named) {
                    syntaxItem += " [[-" + item.Name + "] <" + paramType + ">" + validateSet + "]";
                } else if (!String.IsNullOrEmpty(paramType) && paramType != "SwitchParameter") {
                    syntaxItem += " [-" + item.Name + " <" + paramType + ">" + validateSet + "]";
                } else {
                    syntaxItem += " [-" + item.Name + validateSet + "]";
                }
            }
            Syntax.Add(syntaxItem);
        }
    }

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
        var processed = new List<String>();
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
                processed.Add(Parameters[index].Name);
            } else {
                // saved cmdlet contains orphaned parameter
                Parameters[index].Status = ItemStatus.Missing;
            }
        }
        // process active non-processed parameters. They are new parameters
        foreach (PsCommandParameterVM param in sourceCmdlet.Parameters.Where(param => !processed.Contains(param.Name))) {
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
            SupportInformation = SupportInformation.ToXmlObject(),
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