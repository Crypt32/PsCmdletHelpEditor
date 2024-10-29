using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using CmdletHelpEditor.API.Tools;
using CmdletHelpEditor.API.Utility;
using CmdletHelpEditor.Properties;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Models.Xml;

namespace CmdletHelpEditor.API.Models;

[XmlInclude(typeof(CmdletObject))]
public class ModuleObject : INotifyPropertyChanged, IModuleInfo {
    Boolean useSupports, overridePostCount, useOnlineProvider, isOffline;
    String projectPath, extraHeader, extraFooter;
    Int32? fetchPostCount;
    Double formatVersion;
    ProviderInformation provider;
    CmdletObject selectedCmdlet;
    ObservableCollection<CmdletObject> cmdlets = [];

    void cmdletsOnCollectionChanged(Object Sender, NotifyCollectionChangedEventArgs e) {
        switch (e.Action) {
            case NotifyCollectionChangedAction.Add:
                ((INotifyPropertyChanged)e.NewItems[0]).PropertyChanged += OnPropertyChanged;
                break;
            case NotifyCollectionChangedAction.Remove:
                ((INotifyPropertyChanged)e.OldItems[0]).PropertyChanged -= OnPropertyChanged;
                break;
            case NotifyCollectionChangedAction.Reset:
                if (e.OldItems == null) { break; }
                foreach (INotifyPropertyChanged itemToRemove in e.OldItems) {
                    itemToRemove.PropertyChanged -= OnPropertyChanged;
                }
                break;
        }
        OnPropertyChanged("Blah", true);
    }

    void OnPropertyChanged(Object Sender, PropertyChangedEventArgs ChangedEventArgs) {
        OnPropertyChanged("blah", true);
    }

    [XmlAttribute("fVersion")]
    public Double FormatVersion {
        get => formatVersion;
        set {
            formatVersion = value;
            OnPropertyChanged(nameof(UpgradeRequired));
        }
    }
    public String Name { get; set; }
    [XmlAttribute("type")]
    public ModuleType ModuleType { get; set; }
    public String Version { get; set; }
    public String Description { get; set; }
    [XmlAttribute("mclass")]
    public String ModuleClass { get; set; }
    public String ModulePath { get; set; }
    [XmlAttribute(nameof(useSupports))]
    public Boolean UseSupports {
        get => useSupports;
        set {
            useSupports = value;
            OnPropertyChanged(nameof(UseSupports), true);
        }
    }
    [XmlIgnore]
    public Boolean ImportedFromHelp { get; set; }
    [XmlIgnore]
    public String ProjectPath {
        get => projectPath;
        set {
            projectPath = value;
            OnPropertyChanged();
        }
    }
    [XmlIgnore]
    public Boolean IsOffline {
        get => isOffline;
        set {
            isOffline = value;
            ModuleStatus = isOffline
                ? "Offline"
                : "Online";
            OnPropertyChanged(nameof(ModuleStatus));
            OnPropertyChanged(nameof(UpgradeRequired));
        }
    }
    [XmlIgnore]
    public String ModuleStatus { get; set; }
    [XmlIgnore]
    public Boolean UpgradeRequired => FormatVersion < Utils.CurrentFormatVersion && IsOffline;
    // external information
    [XmlIgnore]
    public Boolean UseOnlineProvider {
        get => useOnlineProvider;
        set {
            useOnlineProvider = value;
            OnPropertyChanged();
        }
    }
    public Boolean HasManifest { get; set; }
    public ProviderInformation Provider {
        get => provider;
        set {
            provider = value;
            UseOnlineProvider = provider != null;
            OnPropertyChanged(nameof(Provider), true);
        }
    }
    public Boolean OverridePostCount {
        get => overridePostCount;
        set {
            overridePostCount = value;
            OnPropertyChanged(nameof(OverridePostCount), true);
        }
    }
    public Int32? FetchPostCount {
        get => fetchPostCount;
        set {
            fetchPostCount = value;
            OnPropertyChanged(nameof(FetchPostCount), true);
        }
    }
    public String ExtraHeader {
        get => extraHeader;
        set {
            extraHeader = value;
            OnPropertyChanged(nameof(ExtraHeader), true);
        }
    }
    public String ExtraFooter {
        get => extraFooter;
        set {
            extraFooter = value;
            OnPropertyChanged(nameof(ExtraFooter), true);
        }
    }
    // editor
    public ObservableCollection<CmdletObject> Cmdlets {
        get => cmdlets;
        set {
            if (cmdlets != null) {
                cmdlets.CollectionChanged -= cmdletsOnCollectionChanged;
            }
            cmdlets = value;
            if (cmdlets != null) {
                cmdlets.CollectionChanged += cmdletsOnCollectionChanged;
            }
        }
    }
    public CmdletObject SelectedCmdlet {
        get => selectedCmdlet;
        set {
            selectedCmdlet = value;
            OnPropertyChanged();
        }
    }

    /// <param name="module">Module imported from project</param>
    /// <param name="cmdlets">active cmdlets from online module</param>
    public void CompareCmdlets(IList<CmdletObject> cmdlets) {
        if (Cmdlets.Count == 0) {
            Cmdlets = new ObservableCollection<CmdletObject>(cmdlets);
            return;
        }
        var processed = new HashSet<String>(StringComparer.OrdinalIgnoreCase);
        // process saved cmdlets
        foreach (CmdletObject cmdlet in Cmdlets) {
            Int32 activeCmdletIndex = cmdlets.IndexOf(cmdlet);
            if (activeCmdletIndex >= 0) {
                // update syntax, parameter sets and parameter information from active cmdlet to project
                cmdlet.Syntax = cmdlets[activeCmdletIndex].Syntax;
                cmdlet.ParameterSets = cmdlets[activeCmdletIndex].ParameterSets;
                cmdlet.UpdateParamSets();
                cmdlet.CopyFromCmdlet(cmdlets[activeCmdletIndex]);
                processed.Add(cmdlet.Name);
            } else {
                // saved project contains orphaned cmdlet
                cmdlet.GeneralHelp.Status = ItemStatus.Missing;
            }
        }
        // add new cmdlets to the project if any
        foreach (CmdletObject cmdlet in cmdlets.Where(cmdlet => !processed.Contains(cmdlet.Name))) {
            Cmdlets.Add(cmdlet);
        }
    }
    public String GetInvocationString(String commandTypes) {
        Object[] args = new Object[5];
        args[1] = Name;
        args[4] = commandTypes;
        if (ModuleClass is "Module" or "External") {
            args[0] = "Import-Module";
            args[2] = String.IsNullOrEmpty(ModulePath)
                ? Name
                : ModulePath;
            args[3] = ModuleClass == "External" || !HasManifest
                ? null
                : " -RequiredVersion " + Version;
            return String.Format(Resources.ipmoTemplate, args);
        }
        args[0] = "Add-PSSnapin";
        args[2] = args[3] = null;
        return String.Format(Resources.ipmoTemplate, args);
    }

    public XmlPsModuleProject ToXmlObject() {
        return new XmlPsModuleProject {
            Name = Name,
            FormatVersion = FormatVersion,
            ModuleType = ModuleType,
            ModuleClass = ModuleClass,
            Version = Version,
            Description = Description,
            ModulePath = ModulePath,
            UseSupports = UseSupports,
            HasManifest = HasManifest,
            OverridePostCount = OverridePostCount,
            FetchPostCount = FetchPostCount,
            ExtraHeader = ExtraHeader,
            ExtraFooter = ExtraFooter,
            Cmdlets = Cmdlets
                .Where(x => x.GeneralHelp.Status != ItemStatus.Missing)
                .Select(x => x.ToXmlObject())
                .ToList(),
            Provider = Provider?.ToXmlObject()
        };
    }

    public static ModuleObject FromPsModuleInfo(PsModuleInfo moduleInfo) {
        return new ModuleObject {
            Name = moduleInfo.Name,
            ModuleType = moduleInfo.ModuleType,
            Version = moduleInfo.Version,
            Description = moduleInfo.Description,
            ModuleClass = moduleInfo.ModuleClass,
            HasManifest = moduleInfo.HasManifest,
            ModulePath = moduleInfo.ModulePath
        };
    }
    public static ModuleObject FromProjectInfo(IPsModuleProject project) {
        return new ModuleObject {
            Name = project.Name,
            FormatVersion = project.FormatVersion,
            ModuleType = project.ModuleType,
            ModuleClass = project.ModuleClass,
            Version = project.Version,
            Description = project.Description,
            ModulePath = project.ModulePath,
            UseSupports = project.UseSupports,
            HasManifest = project.HasManifest,
            ProjectPath = project.ProjectPath,
            Provider = ProviderInformation.FromProviderInfo(project.GetXmlRpcProviderInfo()),
            OverridePostCount = project.OverridePostCount,
            FetchPostCount = project.FetchPostCount,
            ExtraHeader = project.ExtraHeader,
            ExtraFooter = project.ExtraFooter,
            Cmdlets = new ObservableCollection<CmdletObject>(project.GetCmdlets().Select(CmdletObject.FromCommandInfo))
        };
    }

    #region ToString

    public override String ToString() {
        return Name;
    }

    #endregion

    #region Equals

    public override Boolean Equals(Object obj) {
        if (ReferenceEquals(null, obj)) { return false; }
        if (ReferenceEquals(this, obj)) { return true; }
        return obj.GetType() == GetType() && Equals((ModuleObject)obj);
    }
    protected Boolean Equals(ModuleObject other) {
        return String.Equals(Name, other.Name) && String.Equals(Version, other.Version);
    }
    public override Int32 GetHashCode() {
        unchecked {
            Int32 hashCode = Name != null ? Name.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (Version != null ? Version.GetHashCode() : 0);
            return hashCode;
        }
    }

    #endregion

    void OnPropertyChanged([CallerMemberName] String propertyName = null, Boolean markUnsaved = false) {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null) {
            if (markUnsaved) {
                SavePendingEventHandler handler2 = PendingSave;
                handler2?.Invoke(this, new SavePendingEventArgs());
            }
            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;
    public event SavePendingEventHandler PendingSave;
}
