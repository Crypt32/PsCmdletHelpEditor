﻿using System;
using System.Windows.Input;
using PsCmdletHelpEditor.BLL.Abstraction;
using SysadminsLV.WPF.OfficeTheme.Toolkit;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    public class AppConfigVM : ViewModelBase, IAppConfigVM {
        readonly IConfigProvider _configProvider;
        Boolean? dialogResult;

        public AppConfigVM(IConfigProvider configProvider) {
            _configProvider = configProvider;
            SaveConfigCommand = new RelayCommand(saveConfig);
            CloseCommand = new RelayCommand(close);
        }

        public ICommand SaveConfigCommand { get; }
        public ICommand CloseCommand { get; }

        public Boolean LoadPsFunctions {
            get => _configProvider.LoadPsFunctions;
            set {
                _configProvider.LoadPsFunctions = value;
                OnPropertyChanged(nameof(LoadPsFunctions));
            }
        }
        public Boolean LoadPsFilter {
            get => _configProvider.LoadPsFilter;
            set {
                _configProvider.LoadPsFilter = value;
                OnPropertyChanged(nameof(LoadPsFilter));
            }
        }
        public Boolean LoadPsCmdlets {
            get => _configProvider.LoadPsCmdlets;
            set {
                _configProvider.LoadPsCmdlets = value;
                OnPropertyChanged(nameof(LoadPsCmdlets));
            }
        }
        public Boolean LoadPsWorkflows {
            get => _configProvider.LoadPsWorkflows;
            set {
                _configProvider.LoadPsWorkflows = value;
                OnPropertyChanged(nameof(LoadPsWorkflows));
            }
        }
        public Boolean LoadPsApplications {
            get => _configProvider.LoadPsApplications;
            set {
                _configProvider.LoadPsApplications = value;
                OnPropertyChanged(nameof(LoadPsApplications));
            }
        }
        public Boolean LoadPsDscConfigurations {
            get => _configProvider.LoadPsDscConfigurations;
            set {
                _configProvider.LoadPsDscConfigurations = value;
                OnPropertyChanged(nameof(LoadPsDscConfigurations));
            }
        }
        public Boolean LoadExternalPsScripts {
            get => _configProvider.LoadExternalPsScripts;
            set {
                _configProvider.LoadExternalPsScripts = value;
                OnPropertyChanged(nameof(LoadExternalPsScripts));
            }
        }
        public Boolean LoadPsScripts {
            get => _configProvider.LoadPsScripts;
            set {
                _configProvider.LoadPsScripts = value;
                OnPropertyChanged(nameof(LoadPsScripts));
            }
        }
        public Boolean ShowToolbar {
            get => _configProvider.ShowToolbar;
            set {
                _configProvider.ShowToolbar = value;
                OnPropertyChanged(nameof(_configProvider.ShowToolbar));
            }
        }
        public Boolean ShowStatusBar {
            get => _configProvider.ShowStatusBar;
            set {
                _configProvider.ShowStatusBar = value;
                OnPropertyChanged(nameof(ShowStatusBar));
            }
        }
        public Boolean ToolbarLocked {
            get => _configProvider.ToolbarLocked;
            set {
                _configProvider.ToolbarLocked = value;
                OnPropertyChanged(nameof(ToolbarLocked));
            }
        }

        public Boolean? DialogResult {
            get => dialogResult;
            set {
                dialogResult = value;
                OnPropertyChanged(nameof(DialogResult));
            }
        }

        void saveConfig(Object obj) {
            try {
                _configProvider.SaveSettings();
                DialogResult = true;
            } catch (Exception e) {
                MsgBox.Show("Save Settings", $"An error occured during settings save.\n{e.Message}");
            }
        }
        void close(Object o) {
            DialogResult = true;
        }
    }
}