using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.Properties;
using CmdletHelpEditor.Views.UserControls;

namespace CmdletHelpEditor.API.ViewModels;
public class MainWindowVM : DependencyObject, INotifyPropertyChanged, IMainWindowVM {
    Visibility pbVisible;
    Double pbProgress;
    String busyControlText;
    Int32? psVersion;
    ClosableModuleItem selectedTab;
    TabDocumentVM selectedDocument;

    public MainWindowVM(IDataSource dataSource, IProgressBar progressBar) {
        DataSource = dataSource;
        ProgressBar = progressBar;
        DataSource.ModuleList.Add(new PsModuleItem());
        //Settings.Default.Reload();
        CommandManager = new AppCommands(this);
        ConfigContext = new ConfigVM();
        initialize();
    }
    void initialize() {
        Documents.Add(new BlankDocumentVM());
        // TODO: remove
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
    public ObservableCollection<TabDocumentVM> Documents { get; } = [];
    public ObservableCollection<ModuleObject> Modules { get; } = [];
    public ObservableCollection<ClosableModuleItem> Tabs { get; } = [];
    public IDataSource DataSource { get; }
    public IProgressBar ProgressBar { get; }

    // must be dependency property.
    public static readonly DependencyProperty SelectedModuleProperty = DependencyProperty.Register(
        nameof(SelectedModule),
        typeof(ModuleObject),
        typeof(MainWindowVM),
        new PropertyMetadata(null));

    // objects
    public ModuleObject SelectedModule {
        get => (ModuleObject)GetValue(SelectedModuleProperty);
        set => SetValue(SelectedModuleProperty, value);
    }
    public TabDocumentVM SelectedDocument {
        get => selectedDocument;
        set {
            selectedDocument = value;
            OnPropertyChanged(nameof(SelectedDocument));
        }
    }
    public Int32? PsVersion {
        get => psVersion;
        set {
            psVersion = value;
            switch (psVersion) {
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
