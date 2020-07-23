using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.Properties;
using CmdletHelpEditor.Views.UserControls;

namespace CmdletHelpEditor.API.ViewModels {
    public class MainWindowVM : DependencyObject, INotifyPropertyChanged {
        Visibility pbVisible;
        Double pbProgress;
        String busyControlText;
        Int32? psversion;
        ClosableModuleItem selectedTab;

        public MainWindowVM(IDataSource dataSource) {
            DataSource = dataSource;
            DataSource.ModuleList.Add(new PsModuleItem());
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
        public IDataSource DataSource { get; }
        
        // must be dependency property.
        public static readonly DependencyProperty SelectedModuleProperty = DependencyProperty.Register(
            nameof(SelectedModule),
            typeof(ModuleObject),
            typeof(MainWindowVM),
            new PropertyMetadata(null));

        // status bar
        public static readonly DependencyProperty PbVisibleProperty = DependencyProperty.Register(
            nameof(PbVisible),
            typeof(Visibility),
            typeof(MainWindowVM),
            new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty PbProgressProperty = DependencyProperty.Register(
            nameof(PbProgress),
            typeof(Double),
            typeof(MainWindowVM),
            new PropertyMetadata((Double)0));
        public static readonly DependencyProperty BusyControlTextProperty = DependencyProperty.Register(
            nameof(BusyControlText),
            typeof(String),
            typeof(MainWindowVM),
            new PropertyMetadata(String.Empty));
        
        // objects
        public ModuleObject SelectedModule {
            get => (ModuleObject)GetValue(SelectedModuleProperty);
            set => SetValue(SelectedModuleProperty, value);
        }
        public Int32? PsVersion {
            get => psversion;
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
                OnPropertyChanged(nameof(PsVersion));
            }
        }
        public Visibility PbVisible {
            get => pbVisible;
            set {
                pbVisible = value;
                OnPropertyChanged(nameof(PbVisible));
            }
        }
        public Double PbProgress {
            get => pbProgress;
            set {
                pbProgress = value;
                OnPropertyChanged(nameof(PbProgress));
            }
        }
        public String BusyControlText {
            get => busyControlText;
            set {
                busyControlText = value;
                OnPropertyChanged(nameof(BusyControlText));
            }
        }
        public ClosableModuleItem SelectedTab {
            get => selectedTab;
            set {
                selectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
            }
        }

        void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
