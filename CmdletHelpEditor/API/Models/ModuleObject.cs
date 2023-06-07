﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Xml.Serialization;
using CmdletHelpEditor.API.Tools;
using CmdletHelpEditor.API.Utility;
using CmdletHelpEditor.Properties;

namespace CmdletHelpEditor.API.Models {
    [XmlInclude(typeof(CmdletObject))]
    public class ModuleObject : INotifyPropertyChanged {
        Boolean useSupports, overridePostCount, useOnlineProvider, isOffline;
        String projectPath, extraHeader, extraFooter;
        Int32? fetchPostCount;
        Double formatVersion;
        ProviderInformation provider;
        ObservableCollection<CmdletObject> cmdlets;

        public ModuleObject() {
            Cmdlets = new ObservableCollection<CmdletObject>();
        }

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
                OnPropertyChanged(nameof(UpgradeRequired), false);
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
                OnPropertyChanged(nameof(ProjectPath), false);
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
                OnPropertyChanged(nameof(ModuleStatus), false);
                OnPropertyChanged(nameof(UpgradeRequired), false);
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
                OnPropertyChanged(nameof(UseOnlineProvider), false);
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
        public BlogInfo Blog { get; set; }
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

        /// <param name="module">Module imported from project</param>
        /// <param name="cmdlets">active cmdlets from online module</param>
        public void CompareCmdlets(IList<CmdletObject> cmdlets) {
            if (Cmdlets.Count == 0) {
                Cmdlets = new ObservableCollection<CmdletObject>(cmdlets);
                return;
            }
            List<String> processed = new List<String>();
            // process saved cmdlets
            foreach (CmdletObject cmdlet in Cmdlets) {
                Int32 activeCmdletIndex = cmdlets.IndexOf(cmdlet);
                if (activeCmdletIndex >= 0) {
                    // update syntax, parametersets and parameter information from active cmdlet to project
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
            if (ModuleClass == "Module" || ModuleClass == "External") {
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

        protected Boolean Equals(ModuleObject other) {
            return String.Equals(Name, other.Name) && String.Equals(Version, other.Version);
        }

        public override String ToString() {
            return Name;
        }
        public override Int32 GetHashCode() {
            unchecked {
                Int32 hashCode = Name != null ? Name.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Version != null ? Version.GetHashCode() : 0);
                return hashCode;
            }
        }
        public override Boolean Equals(Object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((ModuleObject) obj);
        }

        void OnPropertyChanged(String name, Boolean markUnsaved) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                if (markUnsaved) {
                    SavePendingEventHandler handler2 = PendingSave;
                    handler2?.Invoke(this, new SavePendingEventArgs());
                }
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public event SavePendingEventHandler PendingSave;
    }
}
