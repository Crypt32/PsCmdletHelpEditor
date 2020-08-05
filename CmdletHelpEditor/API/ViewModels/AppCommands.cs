using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.API.Tools;
using CmdletHelpEditor.Views.UserControls;
using CmdletHelpEditor.Views.Windows;
using Microsoft.Win32;
using SysadminsLV.WPF.OfficeTheme.Toolkit;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using Unity;

namespace CmdletHelpEditor.API.ViewModels {
    public class AppCommands {
        readonly MainWindowVM _mwvm;
        readonly IPsProcessor _psProcessor;
        Boolean alreadyRaised;

        public AppCommands(MainWindowVM parent) {
            _psProcessor = App.Container.Resolve<IPsProcessor>();
            _mwvm = parent;
            AddTabCommand = new RelayCommand(addTab);
            CloseTabCommand = new RelayCommand(closeTab);
            LoadModulesCommand = new AsyncCommand(loadModules, canLoadModuleList);
            LoadFromFileCommand = new AsyncCommand(loadModuleFromFile, canLoadModuleList);
            ImportFromXmlHelpCommand = new RelayCommand(importFromXmlHelp, canImportFromHelp);
            PublishHelpCommand = new RelayCommand(publishHelpFile, canPublish);
            ImportFromCBHelpCommand = new RelayCommand(importFromCommentHelp, canImportFromHelp);
            NewCommand = new RelayCommand(newProject, canOpen);
            OpenCommand = new RelayCommand(OpenProject, canOpen);
            SaveCommand = new RelayCommand(saveProjectFile, canSave);
            CloseAppCommand = new RelayCommand<CancelEventArgs>(closeApp);
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
        void addTab(Object obj) {
            ClosableModuleItem tab = UIManager.GenerateTab();
            _mwvm.Tabs.Add(tab);
            tab.Focus();
        }
        void closeTab(Object obj) {
            if (!(obj is ClosableModuleItem)) { return; }
            if (_mwvm.SelectedTab.IsSaved) {
                _mwvm.Tabs.Remove((ClosableModuleItem)obj);
            } else {
                if (testSaved((ClosableModuleItem)obj)) {
                    _mwvm.Tabs.Remove((ClosableModuleItem)obj);
                }
            }
        }
        // toolbar/menu commands
        void newProject(Object obj) {
            if (!testSaved(_mwvm.SelectedTab)) {
                return;
            }
            if (_mwvm.SelectedTab == null) {
                addTab(null);
            }

            ClosableModuleItem tab = _mwvm.SelectedTab;
            Debug.Assert(tab != null, "tab != null");
            tab.Module = null;
            LoadModulesCommand.Execute(true);
            tab.Header = "untitled";
        }
        void saveProjectFile(Object obj) {
            String path;
            // save
            if (obj == null) {
                if (!String.IsNullOrEmpty(_mwvm.SelectedTab.Module.ProjectPath)) {
                    path = _mwvm.SelectedTab.Module.ProjectPath;
                } else {
                    if (!getSaveFileName(out path)) { return; }
                }
            } else {
                // save as
                if (!getSaveFileName(out path)) { return; }
            }
            _mwvm.SelectedTab.Header = (new FileInfo(path)).Name;
            try {
                FileProcessor.SaveProjectFile(_mwvm.SelectedTab, path);
            } catch (Exception e) {
                MsgBox.Show("Save error", e.Message);
                _mwvm.SelectedTab.ErrorInfo = e.Message;
            }
        }
        void closeApp(CancelEventArgs e) {
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
        void importFromXmlHelp(Object obj) {
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
        void importFromCommentHelp(Object obj) {
            LoadCmdlets(null, true);
        }
        void publishHelpFile(Object obj) {
            Object[] param = (Object[])obj;
            ModuleObject module = ((ClosableModuleItem)param[0]).Module;
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
            OnlinePublishProgressWindow dlg = new OnlinePublishProgressWindow(_mwvm.SelectedTab.Module);
            dlg.Show();
        }

        async Task loadModules(Object o, CancellationToken token) {
            // method call from ICommand is allowed only when module selector is active
            // so skip checks.
            ClosableModuleItem previousTab = _mwvm.SelectedTab;
            UIManager.ShowBusy(previousTab, Strings.InfoModuleListLoading);
            _mwvm.Modules.Clear();
            try {
                await _psProcessor.EnumModules(o == null);

                foreach (ModuleObject item in _psProcessor.ModuleList) {
                    _mwvm.Modules.Add(item);
                }
            } catch (Exception e) {
                MsgBox.Show("Error", e.Message);
                previousTab.ErrorInfo = e.Message;
            }
            UIManager.ShowModuleList(previousTab);
        }
        async Task loadModuleFromFile(Object o, CancellationToken token) {
            // method call from ICommand is allowed only when module selector is active
            // so skip checks.
            ClosableModuleItem previousTab = _mwvm.SelectedTab;
            OpenFileDialog dlg = new OpenFileDialog {
                DefaultExt = ".psm1",
                Filter = "PowerShell module files (*.psm1, *.psd1)|*.psm1;*.psd1"
            };
            Boolean? result = dlg.ShowDialog();
            if (result != true) { return; }
            UIManager.ShowBusy(previousTab, Strings.InfoModuleLoading);
            try {
                ModuleObject module = await _psProcessor.GetModuleFromFile(dlg.FileName);
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
        async Task loadCmdletsForProject(ClosableModuleItem tab) {
            String cmd = Utils.GetCommandTypes();
            if (String.IsNullOrEmpty(cmd)) {
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
                IEnumerable<CmdletObject> data = await _psProcessor.EnumCmdlets(tab.Module, cmd, false);
                nativeCmdlets.AddRange(data);
                tab.Module.CompareCmdlets(nativeCmdlets);
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
        Boolean canOpen(Object obj) {
            return _mwvm.SelectedTab == null ||
                (((Grid)_mwvm.SelectedTab.Content).Children.Count == 0 ||
                !(((Grid)_mwvm.SelectedTab.Content).Children[0] is BusyUC));
        }
        Boolean canSave(Object obj) {
            return _mwvm.SelectedTab != null &&
                   _mwvm.SelectedTab.Module != null &&
                  !_mwvm.SelectedTab.Module.UpgradeRequired;
        }
        Boolean canLoadModuleList(Object obj) {
            return _mwvm.SelectedTab != null && ((Grid)_mwvm.SelectedTab.Content).Children[0] is ModuleSelectorControl;
        }
        static Boolean canImportFromHelp(Object obj) {
            return obj != null;
        }
        static Boolean canPublish(Object obj) {
            Object[] param = (Object[])obj;
            return ((ClosableModuleItem) param[0])?.Module != null && ((ClosableModuleItem)param[0]).Module.Cmdlets.Count > 0;
        }
        Boolean canPublishOnline(Object obj) {
            return _mwvm.SelectedTab != null && _mwvm.SelectedTab.Module != null && _mwvm.SelectedTab.Module.Provider != null;
        }

        // utility
        Boolean testSaved(ClosableModuleItem tab) {
            if (tab == null || tab.IsSaved || tab.Module == null) { return true; }
            tab.Focus();
            MessageBoxResult mbxResult = MsgBox.Show("PS Cmdlet Help Editor", Strings.InfoSaveRequired, MessageBoxImage.Warning, MessageBoxButton.YesNoCancel);
            switch (mbxResult) {
                case MessageBoxResult.Yes: saveProjectFile(null); return true;
                case MessageBoxResult.No: return true;
                case MessageBoxResult.Cancel: return false;
            }
            return true;
        }
        Boolean getSaveFileName(out String path) {
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

        // public
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
            addTab(null);
            ClosableModuleItem tab = _mwvm.SelectedTab;
            UIManager.ShowBusy(tab, Strings.InfoCmdletsLoading);
            try {
                ModuleObject module = FileProcessor.ReadProjectFile(fileName);
                Debug.Assert(tab != null, "tab != null");
                tab.Module = module;
                loadCmdletsForProject(tab);
            } catch (Exception e) {
                MsgBox.Show("Read error", e.Message);
            }
        }
        public async void LoadCmdlets(Object helpPath, Boolean importCBH) {
            ClosableModuleItem previousTab = _mwvm.SelectedTab;
            UIElement previousElement = ((Grid)previousTab.Content).Children[0];
            String cmd = Utils.GetCommandTypes();
            if (String.IsNullOrEmpty(cmd)) {
                MsgBox.Show("Error", Strings.E_EmptyCmds);
                return;
            }
            UIManager.ShowBusy(previousTab, Strings.InfoCmdletsLoading);
            try {
                IEnumerable<CmdletObject> data = await _psProcessor.EnumCmdlets(_mwvm.SelectedModule, cmd, importCBH);
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
    }
}
