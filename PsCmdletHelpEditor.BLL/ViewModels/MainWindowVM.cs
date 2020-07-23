using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using PsCmdletHelpEditor.BLL.Abstraction;
using PsCmdletHelpEditor.BLL.Models;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    public class MainWindowVM : DependencyObject, INotifyPropertyChanged, IMainWindowVM {
        Visibility pbVisible;
        Double pbProgress;
        String busyControlText;
        Int32? psversion;
        TabItem selectedTab;

        public MainWindowVM(
            ITabItemManagerVM tabManager,
            IFormatCommands formatCommands,
            IAppConfigVM appConfig,
            IProgressStatus progress) {
            ProgressStatus = progress;
            TabManager = tabManager;
            FormatCommands = formatCommands;
            CommandManager = new AppCommands(this);
            ConfigContext = appConfig;
            initialize();
        }
        void initialize() {
            //Panel content = new Grid();
            //ClosableModuleItem cti = new ClosableModuleItem {
            //    Header = "untitled",
            //    IsSaved = true,
            //    IsClosable = true,
            //    Content = content
            //};
            //content.Children.Add(new StartUserControl());
            //cti.Content = content;
            //cti.EditorContext = new EditorVM(cti);
            //Tabs.Add(cti);
            //SelectedTab = cti;
        }

        #region External properties
        public AppCommands CommandManager { get; set; }
        public EditorVM EditorContext { get; set; }
        public ParamVM ParamContext { get; set; }
        public RelatedLinkVM RelatedLinkContext { get; set; }
        public ExampleVM ExampleContext { get; set; }
        public OutputVM OutputContext { get; set; }
        public IAppConfigVM ConfigContext { get; }
        public IFormatCommands FormatCommands { get; }
        #endregion

        // data definitions
        public ITabItemManagerVM TabManager { get; }
        public ObservableCollection<PsModuleObject> Modules { get; }
            = new ObservableCollection<PsModuleObject>();
        public ObservableCollection<TabItem> Tabs { get; }
            = new ObservableCollection<TabItem>();

        // must be dependency property.
        public static readonly DependencyProperty SelectedModuleProperty = DependencyProperty.Register(
            nameof(SelectedModule),
            typeof(PsModuleObject),
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

        public IProgressStatus ProgressStatus { get; }

        // objects
        public PsModuleObject SelectedModule {
            get => (PsModuleObject)GetValue(SelectedModuleProperty);
            set => SetValue(SelectedModuleProperty, value);
        }
        public Int32? PsVersion {
            get => psversion;
            set {
                psversion = value;
                switch (psversion) {
                    case 3:
                        ConfigContext.SupportsWorkflow = true;
                        break;
                    case 4:
                        ConfigContext.SupportsWorkflow = true;
                        ConfigContext.SupportsDsc = true;
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
        public TabItem SelectedTab {
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
