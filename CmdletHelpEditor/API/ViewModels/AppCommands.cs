#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.API.Tools;
using CmdletHelpEditor.API.Utility;
using CmdletHelpEditor.Views.Windows;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Services;
using SysadminsLV.WPF.OfficeTheme.Controls;
using SysadminsLV.WPF.OfficeTheme.Toolkit;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using Unity;

namespace CmdletHelpEditor.API.ViewModels;

public class AppCommands {
    readonly MainWindowVM _mwvm;
    readonly IPsProcessorLegacy _psProcessorLegacy;
    readonly IPowerShellProcessor _psProcessor;
    readonly IPsHelpProjectFileHandler _fileService;
    readonly IProgressBar _progressBar;
    readonly IMsgBox _msgBox;
    Boolean alreadyRaised;

    public AppCommands(MainWindowVM parent) {
        _psProcessorLegacy = App.Container.Resolve<IPsProcessorLegacy>();
        _psProcessor = App.Container.Resolve<IPowerShellProcessor>();
        _fileService = App.Container.Resolve<IPsHelpProjectFileHandler>();
        _progressBar = App.Container.Resolve<IProgressBar>();
        _mwvm = parent;

        NewCommand = new RelayCommand(addTab);
        OpenCommand = new AsyncCommand(openProject);
        SaveCommand = new RelayCommand(saveProjectFile, canSave);
        CloseTabCommand = new RelayCommand(closeTab, canCloseTab);
        CloseAppCommand = new RelayCommand<CancelEventArgs>(closeApp);
        LoadModulesCommand = new AsyncCommand(loadModules);
        LoadFromFileCommand = new AsyncCommand(loadModuleFromManifest);
        ImportFromMamlCommand = new AsyncCommand(importFromMamlHelp, canImportFromHelp);
        ImportFromCommentBasedHelpCommand = new AsyncCommand(importFromCommentBasedHelp, canImportFromHelp);
        PublishHelpCommand = new AsyncCommand(publishHelpFile, canPublish);
        PublishOnlineCommand = new RelayCommand(publishOnline, canPublishOnline);
        _msgBox = App.Container.Resolve<IMsgBox>();
    }

    #region New Tab command

    public ICommand NewCommand { get; }
    void addTab(Object? o) {
        var vm = new BlankDocumentVM();
        _mwvm.Documents.Add(vm);
        _mwvm.SelectedDocument = vm;
    }

    #endregion

    #region Open Project command

    public IAsyncCommand OpenCommand { get; }

    async Task openProject(Object? obj, CancellationToken token) {
        if (getOpenProjectFilePath(obj as String, out String? projectPath)) {
            HelpProjectDocument? vm = null;
            try {
                IPsModuleProject project = _fileService.ReadProjectFile(projectPath);
                vm = new HelpProjectDocument {
                    Path = projectPath,
                    Module = ModuleObject.FromProjectInfo(project)
                };
                vm.StartSpinner();
            } catch (Exception ex) {
                _msgBox.ShowError("Read error", ex.Message);
            }
            if (_mwvm.SelectedDocument is not BlankDocumentVM) {
                addTab(null);
            }
            swapTabDocument(vm);
            await loadCmdletsForProject2(vm);
            vm.StopSpinner();
        }
    }
    async Task loadCmdletsForProject2(HelpProjectDocument tab) {
        String cmd = Utils.GetCommandTypesString();
        if (String.IsNullOrEmpty(cmd)) {
            _msgBox.ShowError("Error", Strings.E_EmptyCmds);
            return;
        }
        if (_psProcessor.TestModuleExist(tab.Module.Name)) {
            tab.Module.ModulePath = null;
        }
        tab.ErrorInfo = null;
        tab.EditorContext.CurrentCmdlet = null;
        var nativeCmdlets = new List<CmdletObject>();
        try {
            IEnumerable<CmdletObject> data = await _psProcessorLegacy.EnumCmdletsAsync(tab.Module, cmd, false);
            nativeCmdlets.AddRange(data);
            tab.Module.CompareCmdlets(nativeCmdlets);
        } catch (Exception e) {
            String message = $"""
                              {e.Message}

                              You still can use the module project in offline mode
                              However certain functionality may not be available.

                              """;
            _msgBox.ShowError("Error while loading cmdlets", message);
            tab.ErrorInfo = e.Message;
            foreach (CmdletObject cmdlet in tab.Module.Cmdlets) {
                cmdlet.GeneralHelp.Status = ItemStatus.Missing;
            }
        }
    }

    #endregion

    public ICommand SaveCommand { get; }

    #region Close Tab(s) commands
    public ICommand CloseTabCommand { get; }

    void closeTab(Object? o) {
        if (o is null) {
            closeSpecifiedTab(_mwvm.SelectedDocument);
        } else if (o is ClosableTabItem tabItem) { // TODO: need to eliminate explicit reference to UI elements
            var vm = (TabDocumentVM)tabItem.Content;
            closeSpecifiedTab(vm);
        }
    }
    Boolean canCloseTab(Object o) {
        // TODO: need to eliminate explicit reference to UI elements
        return o is null or ClosableTabItem;
    }
    void closeAllTabs(Object o) {
        CloseAllTabs();
    }
    void closeAllButThisTab(Object? o) {
        if (o == null) {
            closeTabsWithPreservation(_mwvm.SelectedDocument);
        } else if (o is ClosableTabItem tabItem) { // TODO: need to eliminate explicit reference to UI elements
            var vm = (TabDocumentVM)tabItem.Content;
            closeTabsWithPreservation(vm);
        }
    }
    Boolean canCloseAllButThisTab(Object? o) {
        if (_mwvm.Documents.Count == 0) {
            return false;
        }
        if (o == null) {
            return _mwvm.SelectedDocument != null;
        }

        return true;
    }

    void closeSpecifiedTab(TabDocumentVM tab) {
        if (!tab.IsModified) {
            _mwvm.Documents.Remove(tab);
        }
        if (tab.IsModified && RequestFileSave(tab)) {
            _mwvm.Documents.Remove(tab);
        }
    }
    Boolean closeTabsWithPreservation(TabDocumentVM? preservedTab = null) {
        // loop over a copy of tabs since we are going to update source collection in a loop
        var tabs = _mwvm.Documents.ToList();
        foreach (TabDocumentVM tab in tabs) {
            if (preservedTab != null && Equals(tab, preservedTab)) {
                continue;
            }
            if (!tab.IsModified) {
                _mwvm.Documents.Remove(tab);

                continue;
            }
            _mwvm.SelectedDocument = tab;
            if (!RequestFileSave(tab)) {
                return false;
            }
            _mwvm.Documents.Remove(tab);
        }

        return true;
    }
    public Boolean CloseAllTabs() {
        return closeTabsWithPreservation();
    }

    #endregion

    #region Close App

    public ICommand CloseAppCommand { get; }
    void closeApp(CancelEventArgs? e) {
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

    #endregion

    #region Load Modules commands

    public IAsyncCommand LoadModulesCommand { get; }

    Task loadModules(Object? o, CancellationToken token) {
        var vm = new ModuleListDocument {
            MWVM = _mwvm
        };
        swapTabDocument(vm);
        return vm.ReloadModules(true);
    }
    

    #endregion

    public ICommand LoadFromFileCommand { get; }
    public ICommand PublishHelpCommand { get; }

    #region Copy To Clipboard command

    public static ICommand CopyToClipCommand => new RelayCommand(copyToClipboard, canCopyClipboard);
    static void copyToClipboard(Object? obj) {
        if (obj == null) { return; }
        Clipboard.SetText((String)obj);
    }
    static Boolean canCopyClipboard(Object? obj) {
        return obj != null;
    }

    #endregion

    public ICommand PublishOnlineCommand { get; }

    
    void addTabOld(Object? obj) {
        ClosableModuleItem tab = UIManager.GenerateTab();
        _mwvm.Tabs.Add(tab);
        tab.Focus();
    }

    // toolbar/menu commands
    public Boolean RequestFileSave(TabDocumentVM tab) {
        MessageBoxResult result = MsgBox.Show(
            "Unsaved Data",
            "Current file was modified. Save changes?",
            MessageBoxImage.Warning,
            MessageBoxButton.YesNoCancel);
        switch (result) {
            case MessageBoxResult.No:
                return true;
            case MessageBoxResult.Yes:
                return writeFile(tab);
            default:
                return false;
        }
    }
    // TODO: implement this
    Boolean writeFile(TabDocumentVM tab) {
        return true;
    }
    void saveProjectFile(Object? obj) {
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
        _mwvm.SelectedTab.Header = new FileInfo(path).Name;
        try {
            FileProcessor.SaveProjectFile(_mwvm.SelectedTab, path);
        } catch (Exception e) {
            _msgBox.ShowError("Save error", e.Message);
            _mwvm.SelectedTab.ErrorInfo = e.Message;
        }
    }

    #region Import external help (MAML, CBH)

    public IAsyncCommand ImportFromMamlCommand { get; }
    public IAsyncCommand ImportFromCommentBasedHelpCommand { get; }
    Task importFromMamlHelp(Object? o, CancellationToken token) {
        var module = (ModuleObject)o!;
        var dlg = NativeDialogFactory.CreateSaveHelpAsXmlDialog(module.Name);
        Boolean? result = dlg.ShowDialog();
        if (result == true) {
        } else {
            return Task.CompletedTask;
        }
        return LoadCommandsAsync(dlg.FileName, false);
    }
    Task importFromCommentBasedHelp(Object? o, CancellationToken token) {
        return LoadCommandsAsync(null, true);
    }
    static Boolean canImportFromHelp(Object? obj) {
        return obj != null;
    }
    
    #endregion

    async Task publishHelpFile(Object o, CancellationToken token) {
        ModuleObject module = ((ClosableModuleItem)o).Module;
        var dlg = NativeDialogFactory.CreateSaveHelpAsXmlDialog(_mwvm.SelectedTab.Module.Name);
        Boolean? result = dlg.ShowDialog();
        if (result == true) {
            try {
                await module.PublishHelpFile(dlg.FileName, _progressBar);
            } catch (Exception e) {
                _msgBox.ShowError("XML Write error", e.Message);
            }
        }
    }
    void publishOnline(Object? obj) {
        var dlg = new OnlinePublishProgressWindow(_mwvm.SelectedTab.Module);
        dlg.Show();
    }


    async Task loadModuleFromFile(Object? o, CancellationToken token) {
        // method call from ICommand is allowed only when module selector is active
        // so skip checks.
        ClosableModuleItem previousTab = _mwvm.SelectedTab;
        var dlg = NativeDialogFactory.CreateOpenPsManifestDialog();
        Boolean? result = dlg.ShowDialog();
        if (result != true) { return; }
        UIManager.ShowBusy(previousTab, Strings.InfoModuleLoading);
        try {
            ModuleObject module = await _psProcessorLegacy.GetModuleFromFileAsync(dlg.FileName);
            if (module != null && !_mwvm.Modules.Contains(module)) {
                _mwvm.Modules.Add(module);
                module.ModulePath = dlg.FileName;
            }
        } catch (Exception e) {
            _msgBox.ShowError("Import error", e.Message);
            previousTab.ErrorInfo = e.Message;
        }
        UIManager.ShowModuleList(previousTab);
    }
    async Task loadModuleFromManifest(Object? o, CancellationToken token) {
        await LoadModulesCommand.ExecuteAsync(false);
        TabDocumentVM selectedDocument = _mwvm.SelectedDocument!;
        var dlg = NativeDialogFactory.CreateOpenPsManifestDialog();
        Boolean? result = dlg.ShowDialog();
        if (result != true) {
            return;
        }
        selectedDocument.StartSpinner(Strings.InfoModuleLoading);
        try {
            PsModuleInfo moduleInfo = await _psProcessor.GetModuleInfoFromFileAsync(dlg.FileName);
            if (!ModuleListDocument.ModuleList.Any(x => x.Name.Equals(moduleInfo.Name))) {
                ModuleListDocument.ModuleList.Add(moduleInfo);
            }
        } catch (Exception e) {
            _msgBox.ShowError("Import error", e.Message);
            //previousTab.ErrorInfo = e.Message;
        }
        selectedDocument.StopSpinner();
    }
    Boolean canLoadModuleList(Object? obj) {
        return false;
        //return ((Grid)_mwvm.SelectedTab?.Content)?.Children[0] is ModuleSelectorControl;
    }
    async Task loadCmdletsForProject(ClosableModuleItem tab) {
        String cmd = Utils.GetCommandTypesString();
        if (String.IsNullOrEmpty(cmd)) {
            _msgBox.ShowError("Error", Strings.E_EmptyCmds);
            return;
        }
        if (_psProcessor.TestModuleExist(tab.Module.Name)) {
            tab.Module.ModulePath = null;
        }
        tab.ErrorInfo = null;
        tab.EditorContext.CurrentCmdlet = null;
        var nativeCmdlets = new List<CmdletObject>();
        try {
            IEnumerable<CmdletObject> data = await _psProcessorLegacy.EnumCmdletsAsync(tab.Module, cmd, false);
            nativeCmdlets.AddRange(data);
            tab.Module.CompareCmdlets(nativeCmdlets);
        } catch (Exception e) {
            String message = $"""
                              {e.Message}

                              You still can use the module project in offline mode
                              However certain functionality may not be available.

                              """;
            _msgBox.ShowError("Error while loading cmdlets", message);
            tab.ErrorInfo = e.Message;
            foreach (CmdletObject cmdlet in tab.Module.Cmdlets) {
                cmdlet.GeneralHelp.Status = ItemStatus.Missing;
            }
        } finally {
            UIManager.ShowEditor(tab);
        }
    }
    

    // predicate
    Boolean canSave(Object? obj) {
        return _mwvm.SelectedDocument is HelpProjectDocument;
        //return _mwvm.SelectedTab is { Module.UpgradeRequired: false };
    }

    
    static Boolean canPublish(Object? obj) {
        return ((ClosableModuleItem)obj)?.Module != null && ((ClosableModuleItem)obj).Module.Cmdlets.Count > 0;
    }
    Boolean canPublishOnline(Object obj) {
        return _mwvm.SelectedTab?.Module?.Provider != null;
    }

    // utility
    Boolean testSaved(ClosableModuleItem? tab) {
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
    Boolean testSaved2(HelpProjectDocument? tab) {
        if (tab is not { IsModified: true } || tab.Module == null) {
            return true;
        }
        _mwvm.SelectedDocument = tab;
        MessageBoxResult mbxResult = MsgBox.Show("PS Cmdlet Help Editor", Strings.InfoSaveRequired, MessageBoxImage.Warning, MessageBoxButton.YesNoCancel);
        switch (mbxResult) {
            case MessageBoxResult.Yes:
                saveProjectFile(null);
                return true;
            case MessageBoxResult.No:
                return true;
            case MessageBoxResult.Cancel: return false;
        }
        return true;
    }
    Boolean getSaveFileName(out String? path) {
        var dlg = NativeDialogFactory.CreateSaveHelpProjectDialog(_mwvm.SelectedTab.Module.Name);
        Boolean? result = dlg.ShowDialog();
        if (result == true) {
            path = dlg.FileName;

            return true;
        }
        path = null;

        return false;
    }

    // public
    void OpenProject2(Object? obj) {
        String fileName;
        
        if (obj == null) {
            var dlg = NativeDialogFactory.CreateOpenHelpProjectDialog();
            Boolean? result = dlg.ShowDialog();
            if (result != true) {
                return;
            }
            fileName = dlg.FileName;
        } else {
            fileName = (String)obj;
        }
        addTabOld(null);
        ClosableModuleItem? tab = _mwvm.SelectedTab;
        UIManager.ShowBusy(tab, Strings.InfoCmdletsLoading);
        try {
            ModuleObject module = ModuleObject.FromProjectInfo(_fileService.ReadProjectFile(fileName));
            tab!.Module = module;
            loadCmdletsForProject(tab);
        } catch (Exception e) {
            _msgBox.ShowError("Read error", e.Message);
        }
    }
    public void OpenProject(String path) {
        OpenCommand.ExecuteAsync(path);
    }

    public async Task LoadCommandsAsync(String? helpPath, Boolean importFromCBH) {
        var vm = new HelpProjectDocument();
        var cmd = Utils.GetCommandTypes();
        if (cmd.Count == 0) {
            _msgBox.ShowError("Error", Strings.E_EmptyCmds);
            return;
        }

        try {
            var moduleInfo = ((ModuleListDocument)_mwvm.SelectedDocument!).SelectedModule;
            ModuleObject? module = ModuleObject.FromPsModuleInfo(moduleInfo);
            IEnumerable<IPsCommandInfo> data = await _psProcessor.EnumCommandsAsync(moduleInfo, cmd, importFromCBH);
            foreach (IPsCommandInfo commandInfo in data) {
                module.Cmdlets.Add(CmdletObject.FromCommandInfo(commandInfo));
            }
            if (helpPath != null) {
                module.ImportedFromHelp = true;
                try {
                    XmlProcessor.ImportFromMaml(helpPath, module);
                } catch (Exception ex) {
                    _msgBox.ShowError("Error", ex.Message);
                }
            }
            vm.Module = module;
            swapTabDocument(vm);
        } catch (Exception ex) {
            _msgBox.ShowError("Error while loading cmdlets", ex.Message);
            _mwvm.SelectedDocument!.ErrorInfo = ex.Message;
        }
    }

    #region other functions

    void swapTabDocument(TabDocumentVM newDocument) {
        if (_mwvm.SelectedDocument is null or HelpProjectDocument) {
            _mwvm.Documents.Add(newDocument);
        } else {
            Int32 index = _mwvm.Documents.IndexOf(_mwvm.SelectedDocument);
            _mwvm.Documents[index] = newDocument;
        }
        _mwvm.SelectedDocument = newDocument;
    }
    static Boolean getOpenProjectFilePath(String? suggestedPath, out String? path) {
        if (suggestedPath is not null) {
            path = suggestedPath;
            return true;
        }
        path = null;
        var dlg = NativeDialogFactory.CreateOpenHelpProjectDialog();
        Boolean? result = dlg.ShowDialog();
        if (result == true) {
            path = dlg.FileName;
        }

        return result ?? false;
    }

    #endregion
}