using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.Properties;
using CmdletHelpEditor.UI.UserControls;

namespace CmdletHelpEditor.API.ViewModels {
	public class MainWindowVM : DependencyObject, INotifyPropertyChanged {
        Visibility pbVisible;
        Double pbProgress;
        String busyControlText;
        Int32? psversion;
		ClosableModuleItem selectedTab;

        public MainWindowVM() {
			//Settings.Default.Reload();
			Modules = new ObservableCollection<ModuleObject>();
			Tabs = new ObservableCollection<ClosableModuleItem>();
			CommandManager = new AppCommands(this);
			ConfigContext = new ConfigVM();
			initialize();
		}
		void initialize() {
			Panel content = new Grid();
			ClosableModuleItem cti = new ClosableModuleItem {
				Header = "untitled",
				IsSaved = true,
				IsClosable = true,
				Content = content
			};
			content.Children.Add(new StartUserControl());
			cti.Content = content;
			cti.EditorContext = new EditorVM(cti);
			Tabs.Add(cti);
			SelectedTab = cti;
		}

		#region External properties
		public AppCommands CommandManager { get; set; }
		public EditorVM EditorContext { get; set; }
		public ParamVM ParamContext { get; set; }
		public RelatedLinkVM RelatedLinkContext { get; set; }
		public ExampleVM ExampleContext { get; set; }
		public OutputVM OutputContext { get; set; }
		public ConfigVM ConfigContext { get; set; }
		#endregion

		// data definitions
		public ObservableCollection<ModuleObject> Modules { get; set; }
		public ObservableCollection<ClosableModuleItem> Tabs { get; set; }
		
		// must be dependency property.
		public static readonly DependencyProperty SelectedModuleProperty = DependencyProperty.Register(
			"SelectedModule",
			typeof(ModuleObject),
			typeof(MainWindowVM),
			new PropertyMetadata(null));

		// status bar
		public static readonly DependencyProperty PbVisibleProperty = DependencyProperty.Register("PbVisible", typeof(Visibility), typeof(MainWindowVM), new PropertyMetadata(Visibility.Collapsed));
		public static readonly DependencyProperty PbProgressProperty = DependencyProperty.Register("PbProgress", typeof(Double), typeof(MainWindowVM), new PropertyMetadata((Double)0));
		public static readonly DependencyProperty BusyControlTextProperty = DependencyProperty.Register("BusyControlText", typeof(String), typeof(MainWindowVM), new PropertyMetadata(String.Empty));
		
		// objects
		public ModuleObject SelectedModule {
			get { return (ModuleObject)GetValue(SelectedModuleProperty); }
			set { SetValue(SelectedModuleProperty, value); }
		}
		public Int32? PsVersion {
			get { return psversion; }
			set {
				psversion = value;
				switch (psversion) {
					case 3:
						Settings.Default.WorkflowEnabled = true;
						break;
					case 4:
						Settings.Default.WorkflowEnabled = true;
						Settings.Default.ConfigurationEnabled = true;
						break;
				}
				OnPropertyChanged("PsVersion");
			}
		}
		public Visibility PbVisible {
			get { return pbVisible; }
			set {
                pbVisible = value;
                OnPropertyChanged("PbVisible");
			}
		}
		public Double PbProgress {
			get { return pbProgress; }
			set {
                pbProgress = value;
                OnPropertyChanged("PbProgress");
			}
		}
		public String BusyControlText {
			get { return busyControlText; }
			set {
                busyControlText = value;
                OnPropertyChanged("BusyControlText");
			}
		}
		public ClosableModuleItem SelectedTab {
			get { return selectedTab; }
			set {
				selectedTab = value;
				OnPropertyChanged("SelectedTab");
			}
		}

		void OnPropertyChanged(String name) {
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
