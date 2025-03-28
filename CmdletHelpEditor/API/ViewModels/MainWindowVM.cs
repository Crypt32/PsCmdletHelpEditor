﻿#nullable enable
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.Properties;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.ViewModels;
public class MainWindowVM : ViewModelBase, IMainWindowVM {
    Int32? psVersion;
    TabDocumentVM? selectedDocument;

    public MainWindowVM(IProgressBar progressBar) {
        NewTabCommand = new RelayCommand(newTab);
        ProgressBar = progressBar;
        //Settings.Default.Reload();
        CommandManager = new AppCommands(this);
        NewTabCommand.Execute(null);
    }

    public ICommand NewTabCommand { get; }

    #region External properties
    public AppCommands CommandManager { get; set; }
    public EditorVM EditorContext { get; set; }
    public ParamVM ParamContext { get; set; }
    public RelatedLinkVM RelatedLinkContext { get; set; }
    public ExampleVM ExampleContext { get; set; }
    public OutputVM OutputContext { get; set; }
    #endregion

    // data definitions
    public ObservableCollection<TabDocumentVM> Documents { get; } = [];
    public IProgressBar ProgressBar { get; }

    public TabDocumentVM? SelectedDocument {
        get => selectedDocument;
        set {
            selectedDocument = value;
            OnPropertyChanged();
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
            OnPropertyChanged();
        }
    }

    void newTab(Object? o) {
        var vm = new BlankDocumentVM();
        Documents.Add(vm);
        SelectedDocument = vm;
    }
}
