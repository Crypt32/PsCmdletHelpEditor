using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using PsCmdletHelpEditor.BLL.Abstraction;
using PsCmdletHelpEditor.BLL.Models;
using PsCmdletHelpEditor.BLL.Tools;
using SysadminsLV.WPF.OfficeTheme.Toolkit;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using Unity;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    public class AppCommands {
        readonly IAppConfigVM _appConfig;
        readonly IUIWindowDialogService _dialogService;
        readonly MainWindowVM _mwvm;
        Boolean alreadyRaised;

        public AppCommands(MainWindowVM parent) {
            _mwvm = parent;
            _appConfig = UnityConfig.Container.Resolve<IAppConfigVM>();
            _dialogService = UnityConfig.Container.Resolve<IUIWindowDialogService>();
            AddTabCommand = new RelayCommand(AddTab);
            CloseTabCommand = new RelayCommand(CloseTab);
            LoadModulesCommand = new RelayCommand(LoadModules, CanLoadModuleList);
            LoadFromFileCommand = new RelayCommand(LoadModuleFromFile, CanLoadModuleList);
            ImportFromXmlHelpCommand = new RelayCommand(ImportFromXmlHelp, CanImportFromHelp);
            PublishHelpCommand = new RelayCommand(PublishHelpFile, CanPublish);
            ImportFromCBHelpCommand = new RelayCommand(ImportFromCommentHelp, CanImportFromHelp);
            NewCommand = new RelayCommand(NewProject, CanOpen);
            OpenCommand = new RelayCommand(OpenProject, CanOpen);
            SaveCommand = new RelayCommand(SaveProjectFile, CanSave);
            CloseAppCommand = new RelayCommand<CancelEventArgs>(CloseApp);
            PublishOnlineCommand = new RelayCommand(publishOnline, canPublishOnline);
        }

        public ICommand AddTabCommand { get; set; }
        public ICommand CloseTabCommand { get; set; }

        public ICommand NewCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CloseAppCommand { get; set; }
        public ICommand LoadModulesCommand { get; set; }
        public ICommand LoadFromFileCommand { get; set; }
        public ICommand ImportFromXmlHelpCommand { get; set; }
        public ICommand ImportFromCBHelpCommand { get; set; }
        public ICommand PublishHelpCommand { get; set; }
        public static ICommand CopyToClipCommand => new RelayCommand(CopyToClipboard, CanCopyClipboard);
        public ICommand PublishOnlineCommand { get; set; }

        static void CopyToClipboard(Object obj) {
            if (obj == null) { return; }
            Clipboard.SetText((String)obj);
        }
        static Boolean CanCopyClipboard(Object obj) {
            return obj != null;
        }
        void AddTab(Object obj) {
            Models.TabItem tab = UIManager.GenerateTab();
            _mwvm.Tabs.Add(tab);
            tab.Focus();
        }
        void CloseTab(Object obj) {
            if (!(obj is Models.TabItem)) { return; }
            if (_mwvm.SelectedTab.IsSaved) {
                _mwvm.Tabs.Remove((Models.TabItem)obj);
            } else {
                if (testSaved((Models.TabItem)obj)) {
                    _mwvm.Tabs.Remove((Models.TabItem)obj);
                }
            }
        }
        // toolbar/menu commands
        void NewProject(Object obj) {
            if (!testSaved(_mwvm.SelectedTab)) { return; }
            if (_mwvm.SelectedTab == null) { AddTab(null); }
            Models.TabItem tab = _mwvm.SelectedTab;
            Debug.Assert(tab != null, "tab != null");
            tab.Module = null;
            LoadModules(true);
            tab.Header = "untitled";
        }
        public void OpenProject(Object obj) {
            String fileName;
            if (obj == null) {
                OpenFileDialog dlg = new OpenFileDialog {
                    DefaultExt = ".pshproj",
                    Filter = "PowerShell Help Project file (.pshproj)|*.pshproj"
                };
                Boolean? result = dlg.ShowDialog();
                if (result != true) {
                    return;
                }
                fileName = dlg.FileName;
            } else {
                fileName = (String)obj;
            }
            AddTab(null);
            Models.TabItem tab = _mwvm.SelectedTab;
            UIManager.ShowBusy(tab, Strings.InfoCmdletsLoading);
            try {
                ModuleObject module = FileProcessor.ReadProjectFile(fileName);
                Debug.Assert(tab != null, "tab != null");
                tab.Module = module;
                LoadCmdletsForProject(tab);
            } catch (Exception e) {
                MsgBox.Show("Read error", e.Message);

            }
        }
        void SaveProjectFile(Object obj) {
            String path;
            // save
            if (obj == null) {
                if (!String.IsNullOrEmpty(_mwvm.SelectedTab.Module.ProjectPath)) {
                    path = _mwvm.SelectedTab.Module.ProjectPath;
                } else {
                    if (!GetSaveFileName(out path)) { return; }
                }
            } else {
                // save as
                if (!GetSaveFileName(out path)) { return; }
            }
            _mwvm.SelectedTab.Header = (new FileInfo(path)).Name;
            try {
                FileProcessor.SaveProjectFile(_mwvm.SelectedTab, path);
            } catch (Exception e) {
                MsgBox.Show("Save error", e.Message);
                _mwvm.SelectedTab.ErrorInfo = e.Message;
            }
        }
        void CloseApp(CancelEventArgs e) {
            if (e == null) {
                Boolean canClose = _mwvm.Tabs.Where(tab => !tab.IsSaved).All(testSaved);
                if (canClose) {
                    alreadyRaised = true;
                    Application.Current.Shutdown();
                }
            } else {
                if (!alreadyRaised) {
                    e.Cancel = !_mwvm.Tabs.Where(tab => !tab.IsSaved).All(testSaved);
                }
            }
        }
        void ImportFromXmlHelp(Object obj) {
            ModuleObject module = (ModuleObject)obj;
            OpenFileDialog dlg = new OpenFileDialog {
                FileName = module.Name + ".Help.xml",
                DefaultExt = ".xml",
                Filter = "PowerShell Help Xml files (.xml)|*.xml"
            };
            Boolean? result = dlg.ShowDialog();
            if (result == true) {
            } else { return; }
            LoadCmdlets(dlg.FileName, false);
        }
        void ImportFromCommentHelp(Object obj) {
            LoadCmdlets(null, true);
        }
        void PublishHelpFile(Object obj) {
            Object[] param = (Object[])obj;
            ModuleObject module = ((Models.TabItem)param[0]).Module;
            ProgressBar pb = ((MainWindow)param[1]).sb.pb;
            SaveFileDialog dlg = new SaveFileDialog {
                FileName = _mwvm.SelectedTab.Module.Name + ".Help.xml",
                DefaultExt = ".xml",
                Filter = "PowerShell Help Xml files (.xml)|*.xml"
            };
            Boolean? result = dlg.ShowDialog();
            if (result == true) {
                try {
                    FileProcessor.PublishHelpFile(dlg.FileName, module, pb);
                } catch (Exception e) {
                    MsgBox.Show("XML Write error", e.Message);
                }
            }
        }
        void publishOnline(Object obj) {
            var vm = UnityConfig.Container.Resolve<IOnlinePublishProgressVM>();
            vm.SetModule(_mwvm.SelectedTab.Module);
        }

        public async void LoadCmdlets(Object helpPath, Boolean importCBH) {
            Models.TabItem previousTab = _mwvm.SelectedTab;
            UIElement previousElement = ((Grid)previousTab.Content).Children[0];
            String commandsToLoad = _appConfig.GetCommandTypesString();
            if (String.IsNullOrEmpty(commandsToLoad)) {
                MsgBox.Show("Error", Strings.E_EmptyCmds);
                return;
            }
            UIManager.ShowBusy(previousTab, Strings.InfoCmdletsLoading);
            try {
                IEnumerable<CmdletObject> data = await PowerShellProcessor.EnumCmdlets(_mwvm.SelectedModule, commandsToLoad, importCBH);
                _mwvm.SelectedModule.Cmdlets.Clear();
                foreach (CmdletObject item in data) {
                    _mwvm.SelectedModule.Cmdlets.Add(item);
                }
                if (helpPath != null) {
                    _mwvm.SelectedModule.ImportedFromHelp = true;
                    XmlProcessor.ImportFromXml((String)helpPath, _mwvm.SelectedModule);
                }
                previousTab.Module = _mwvm.SelectedModule;
                _mwvm.SelectedModule = null;
                UIManager.ShowEditor(previousTab);
            } catch (Exception e) {
                MsgBox.Show("Error while loading cmdlets", e.Message);
                _mwvm.SelectedTab.ErrorInfo = e.Message;
                UIManager.RestoreControl(previousTab, previousElement);
            }
        }
        async void LoadModules(Object obj) {
            // method call from ICommand is allowed only when module selector is active
            // so skip checks.
            Models.TabItem previousTab = _mwvm.SelectedTab;
            UIManager.ShowBusy(previousTab, Strings.InfoModuleListLoading);
            _mwvm.Modules.Clear();
            try {
                IEnumerable<ModuleObject> data = obj == null
                    ? await PowerShellProcessor.EnumModules(true)
                    : await PowerShellProcessor.EnumModules(false);
                foreach (ModuleObject item in data) {
                    _mwvm.Modules.Add(item);
                }
            } catch (Exception e) {
                MsgBox.Show("Error", e.Message);
                previousTab.ErrorInfo = e.Message;
            }
            UIManager.ShowModuleList(previousTab);
        }
        async void LoadModuleFromFile(Object obj) {
            // method call from ICommand is allowed only when module selector is active
            // so skip checks.
            Models.TabItem previousTab = _mwvm.SelectedTab;
            OpenFileDialog dlg = new OpenFileDialog {
                DefaultExt = ".psm1",
                Filter = "PowerShell module files (*.psm1, *.psd1)|*.psm1;*.psd1"
            };
            Boolean? result = dlg.ShowDialog();
            if (result != true) { return; }
            UIManager.ShowBusy(previousTab, Strings.InfoModuleLoading);
            try {
                ModuleObject module = await PowerShellProcessor.GetModuleFromFile(dlg.FileName);
                if (module != null && !_mwvm.Modules.Contains(module)) {
                    _mwvm.Modules.Add(module);
                    module.ModulePath = dlg.FileName;
                }
            } catch (Exception e) {
                MsgBox.Show("Import error", e.Message);
                previousTab.ErrorInfo = e.Message;
            }
            UIManager.ShowModuleList(previousTab);
        }
        async void LoadCmdletsForProject(Models.TabItem tab) {
            String commandsToLoad = _appConfig.GetCommandTypesString();
            if (String.IsNullOrEmpty(commandsToLoad)) {
                MsgBox.Show("Error", Strings.E_EmptyCmds);
                return;
            }
            if (FileProcessor.FindModule(tab.Module.Name)) {
                tab.Module.ModulePath = null;
            }
            tab.ErrorInfo = null;
            tab.EditorContext.CurrentCmdlet = null;
            List<CmdletObject> nativeCmdlets = new List<CmdletObject>();
            try {
                IEnumerable<CmdletObject> data = await PowerShellProcessor.EnumCmdlets(tab.Module, commandsToLoad, false);
                nativeCmdlets.AddRange(data);
                PowerShellProcessor.CompareCmdlets(tab.Module, nativeCmdlets);
            } catch (Exception e) {
                String message = e.Message + "\n\nYou still can use the module project in offline mode";
                message += "\nHowever certain functionality may not be available.";
                MsgBox.Show("Error while loading cmdlets", message);
                tab.ErrorInfo = e.Message;
                foreach (CmdletObject cmdlet in tab.Module.Cmdlets) {
                    cmdlet.GeneralHelp.Status = ItemStatus.Missing;
                }
            } finally {
                UIManager.ShowEditor(tab);
            }
        }

        // predicate
        Boolean CanOpen(Object obj) {
            return _mwvm.SelectedTab == null ||
                (((Grid)_mwvm.SelectedTab.Content).Children.Count == 0 ||
                !(((Grid)_mwvm.SelectedTab.Content).Children[0] is BusyUC));
        }
        Boolean CanSave(Object obj) {
            return _mwvm.SelectedTab != null &&
                   _mwvm.SelectedTab.Module != null &&
                  !_mwvm.SelectedTab.Module.UpgradeRequired;
        }
        Boolean CanLoadModuleList(Object obj) {
            return _mwvm.SelectedTab != null && ((Grid)_mwvm.SelectedTab.Content).Children[0] is ModuleSelectorControl;
        }
        Boolean CanImportFromHelp(Object obj) {
            return obj != null;
        }
        Boolean CanPublish(Object obj) {
            Object[] param = (Object[])obj;
            return param[0] != null &&
                   ((Models.TabItem)param[0]).Module != null &&
                   ((Models.TabItem)param[0]).Module.Cmdlets.Count > 0;
        }
        Boolean canPublishOnline(Object obj) {
            return _mwvm.SelectedTab != null && _mwvm.SelectedTab.Module != null && _mwvm.SelectedTab.Module.Provider != null;
        }

        // utility
        Boolean testSaved(Models.TabItem tab) {
            if (tab == null || tab.IsSaved || tab.Module == null) { return true; }
            tab.Focus();
            MessageBoxResult mbxResult = MsgBox.Show("PS Cmdlet Help Editor", Strings.InfoSaveRequired, MessageBoxImage.Warning, MessageBoxButton.YesNoCancel);
            switch (mbxResult) {
                case MessageBoxResult.Yes: SaveProjectFile(null); return true;
                case MessageBoxResult.No: return true;
                case MessageBoxResult.Cancel: return false;
            }
            return true;
        }
        Boolean GetSaveFileName(out String path) {
            SaveFileDialog dlg = new SaveFileDialog {
                FileName = _mwvm.SelectedTab.Module.Name + ".Help.pshproj",
                DefaultExt = ".pshproj",
                Filter = "PowerShell Help Project file (.pshproj)|*.pshproj"
            };
            Boolean? result = dlg.ShowDialog();
            if (result == true) {
                path = dlg.FileName;
                return true;
            }
            path = null;
            return false;
        }
    }
}
