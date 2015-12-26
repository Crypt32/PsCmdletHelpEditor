using CmdletHelpEditor.API.MetaWeblog;
using CmdletHelpEditor.API.Tools;
using CmdletHelpEditor.API.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Management.Automation;
using System.Windows;
using System.Xml.Serialization;

namespace CmdletHelpEditor.API.BaseClasses {
	[XmlInclude(typeof(CmdletObject))]
	//[XmlRoot("")]
	public class ModuleObject : INotifyPropertyChanged {
		Boolean useSupports, overridePostCount, useOnlineProvider, isOffline;
		String projectPath, extraHeader, extraFooter;
		Int32? fetchPostCount;
		Double formatVersion;
		ProviderInformation provider;
		MainWindowVM mwvm;

		public ModuleObject() {
			Cmdlets = new ObservableCollection<CmdletObject>();
		}

		[XmlAttribute("fVersion")]
		public Double FormatVersion {
			get { return formatVersion; }
			set {
				formatVersion = value;
				OnPropertyChanged("UpgradeRequired", false);
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
		[XmlAttribute("useSupports")]
		public Boolean UseSupports {
			get { return useSupports; }
			set {
				useSupports = value;
				OnPropertyChanged("UseSupports", true);
			}
		}
		[XmlIgnore]
		public Boolean ImportedFromHelp { get; set; }
		[XmlIgnore]
		public String ProjectPath {
			get { return projectPath; }
			set {
				projectPath = value;
				OnPropertyChanged("ProjectPath", false);
			}
		}
		[XmlIgnore]
		public Boolean IsOffline {
			get { return isOffline; }
			set {
				isOffline = value;
				ModuleStatus = isOffline
					? "Offline"
					: "Online";
				OnPropertyChanged("ModuleStatus", false);
				OnPropertyChanged("UpgradeRequired", false);
			}
		}
		[XmlIgnore]
		public String ModuleStatus { get; set; }
		[XmlIgnore]
		public Boolean UpgradeRequired {
			get {
				return FormatVersion < Utils.CurrentFormatVersion && IsOffline;
			}
		}
		// external information
		[XmlIgnore]
		public Boolean UseOnlineProvider {
			get { return useOnlineProvider; }
			set {
				useOnlineProvider = value;
				OnPropertyChanged("UseOnlineProvider", false);
			}
		}
		public Boolean HasManifest { get; set; }
		public ProviderInformation Provider {
			get { return provider; }
			set {
				provider = value;
				UseOnlineProvider = provider != null;
				OnPropertyChanged("Provider", true);
			}
		}
		public BlogInfo Blog { get; set; }
		public Boolean OverridePostCount {
			get { return overridePostCount; }
			set {
				overridePostCount = value;
				OnPropertyChanged("OverridePostCount", true);
			}
		}
		public Int32? FetchPostCount {
			get { return fetchPostCount; }
			set {
				fetchPostCount = value;
				OnPropertyChanged("FetchPostCount", true);
			}
		}
		public String ExtraHeader {
			get { return extraHeader; }
			set {
				extraHeader = value;
				OnPropertyChanged("ExtraHeader", true);
			}
		}
		public String ExtraFooter {
			get { return extraFooter; }
			set {
				extraFooter = value;
				OnPropertyChanged("ExtraFooter", true);
			}
		}
		// editor
		public ObservableCollection<CmdletObject> Cmdlets { get; set; }

		void OnPropertyChanged(String name, Boolean markUnsaved) {
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) {
				if (markUnsaved) {
					try {
						mwvm = (MainWindowVM)Application.Current.MainWindow.DataContext;
						mwvm.SelectedTab.IsSaved = false;
					} catch { }
				}
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
		protected Boolean Equals(ModuleObject other) {
			return String.Equals(Name, other.Name) && String.Equals(Version, other.Version);
		}

		public override String ToString() {
			return Name;
		}
		public override Int32 GetHashCode() {
			unchecked {
				Int32 hashCode = (Name != null ? Name.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Version != null ? Version.GetHashCode() : 0);
				return hashCode;
			}
		}
		public override Boolean Equals(Object obj) {
			if (ReferenceEquals(null, obj)) { return false; }
			if (ReferenceEquals(this, obj)) { return true; }
			return obj.GetType() == GetType() && Equals((ModuleObject) obj);
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
