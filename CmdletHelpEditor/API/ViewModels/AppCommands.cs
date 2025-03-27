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
using PsCmdletHelpEditor.Core.Services.Formatters;
using SysadminsLV.WPF.OfficeTheme.Controls;
using SysadminsLV.WPF.OfficeTheme.Toolkit;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using Unity;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;

namespace CmdletHelpEditor.API.ViewModels;

public class AppCommands {
    readonly MainWindowVM _mwvm;
    readonly IPowerShellProcessor _psProcessor;
    readonly IPsHelpProjectFileHandler _fileService;
    readonly IProgressBar _progressBar;
    readonly IUIMessenger _uiMessenger;
    Boolean alreadyRaised;

    public AppCommands(MainWindowVM parent) {
        _psProcessor = App.Container.Resolve<IPowerShellProcessor>();
        _fileService = App.Container.Resolve<IPsHelpProjectFileHandler>();
        _progressBar = App.Container.Resolve<IProgressBar>();
        _mwvm = parent;

        OpenProjectCommand = new AsyncCommand(openProject);
        SaveProjectCommand = new RelayCommand(saveProjectFile, canSave);
        CloseTabCommand = new RelayCommand(closeTab, canCloseTab);
        CloseAppCommand = new RelayCommand<CancelEventArgs>(closeApp);
        LoadModulesCommand = new AsyncCommand(loadModules);
        LoadFromFileCommand = new AsyncCommand(loadModuleFromManifest);
        ImportFromMamlCommand = new AsyncCommand(importFromMamlHelp, canImportFromHelp);
        ImportFromCommentBasedHelpCommand = new AsyncCommand(importFromCommentBasedHelp, canImportFromHelp);
        PublishHelpCommand = new AsyncCommand(publishHelpFile, canPublish);
        PublishOnlineCommand = new RelayCommand(publishOnline, canPublishOnline);
        PublishMarkdownCommand = new AsyncCommand(publishMarkdown, canSave);
        _uiMessenger = App.Container.Resolve<IUIMessenger>();
    }

    #region New Project command

    public IAsyncCommand NewProjectCommand => LoadModulesCommand;

    #endregion

    #region Open Project command

    public IAsyncCommand OpenProjectCommand { get; }

    async Task openProject(Object? obj, CancellationToken token) {
        if (getOpenProjectFilePath(obj as String, out String? projectPath)) {
            HelpProjectDocument? vm = null;
            try {
                IPsModuleProject project = _fileService.ReadProjectFile(projectPath);
                vm = new HelpProjectDocument(ModuleObject.FromProjectInfo(project)) {
                    Path = projectPath
                };
                vm.StartSpinner();
            } catch (Exception ex) {
                _uiMessenger.ShowError("Read error", ex.Message);
            }
            if (_mwvm.SelectedDocument is not BlankDocumentVM) {
                _mwvm.NewTabCommand.Execute(null);
            }
            swapTabDocument(vm);
            await loadCommandsForProject(vm);
            vm.StopSpinner();
        }
    }
    async Task loadCommandsForProject(HelpProjectDocument tab) {
        tab.StartSpinner();
        IReadOnlyList<String> cmd = Utils.GetCommandTypes();
        if (cmd.Count == 0) {
            _uiMessenger.ShowError("Error", Strings.E_EmptyCmds);
            return;
        }
        if (!_psProcessor.TestModuleExist(tab.Module.Name)) {
            tab.Module.ModulePath = null;
        }
        tab.ErrorInfo = null;
        tab.EditorContext.CurrentCmdlet = null;
        try {
            List<CmdletObject> data = (await _psProcessor.EnumCommandsAsync(tab.Module, cmd, false))
                .Select(CmdletObject.FromCommandInfo)
                .ToList();
            tab.Module.CompareCmdlets(data);
        } catch (Exception e) {
            String message = $"""
                              {e.Message}

                              You still can use the module project in offline mode
                              However certain functionality may not be available.

                              """;
            _uiMessenger.ShowError("Error while loading cmdlets", message);
            tab.ErrorInfo = e.Message;
            foreach (CmdletObject cmdlet in tab.Module.Cmdlets) {
                cmdlet.GeneralHelp.Status = ItemStatus.Missing;
            }
        }
    }

    #endregion

    public ICommand SaveProjectCommand { get; }

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
        if (o is null) {
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
        if (o is null) {
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

        if (_mwvm.Documents.Count == 0) {
            _mwvm.NewTabCommand.Execute(null);
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
        if (_mwvm.Documents.Count == 0) {
            _mwvm.NewTabCommand.Execute(null);
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
            Boolean canClose = _mwvm.Documents
                .Where(tab => tab.SupportsSave && tab.IsModified)
                .Cast<HelpProjectDocument>()
                .All(testSaved);
            if (canClose) {
                alreadyRaised = true;
                Application.Current.Shutdown();
            }
        } else {
            if (!alreadyRaised) {
                e.Cancel = !_mwvm.Documents
                    .Where(tab => tab.SupportsSave && tab.IsModified)
                    .Cast<HelpProjectDocument>()
                    .All(testSaved);
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
        return vm.ReloadModules(true, _psProcessor);
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

    public IAsyncCommand PublishMarkdownCommand { get; }
    async Task publishMarkdown(Object o, CancellationToken token) {
        if (!_uiMessenger.TryGetBrowseFolderDialog(out String folderPath)) {

        }
        
        var selectedDocument = (HelpProjectDocument)_mwvm.SelectedDocument!;
        selectedDocument.StartSpinner(Strings.InfoModuleLoading);
        try {
            IPsModuleProject proj = selectedDocument.Module.ToXmlObject();
            IHelpOutputFormatter formatter = OutputFormatterFactory.GetMarkdownFormatter();
            foreach (IPsCommandInfo command in proj.GetCmdlets()) {
                String content = await formatter.GenerateViewAsync(command, proj);
                String filePath = Path.Combine(folderPath!, command.Name + ".md");
                File.WriteAllText(filePath, content);
            }
        } catch (Exception ex) {
            _uiMessenger.ShowError("Export error", ex.Message);
        }

        selectedDocument.StopSpinner();
    }


    // toolbar/menu commands
    public Boolean RequestFileSave(TabDocumentVM tab) {
        MessageBoxResult result = MsgBox.Show(
            "Unsaved Data",
            "Current project was modified. Save changes?",
            MessageBoxImage.Warning,
            MessageBoxButton.YesNoCancel);
        switch (result) {
            case MessageBoxResult.No:
                return true;
            case MessageBoxResult.Yes:
                return writeFile(tab as HelpProjectDocument);
            default:
                return false;
        }
    }

    void writeFile(HelpProjectDocument? helpProject, String path) {
        if (helpProject is null || String.IsNullOrEmpty(path)) {
            return;
        }

        _fileService.SaveProjectFile(helpProject.Module.ToXmlObject(), path);
        helpProject.Module.RemoveInvalid();
        helpProject.Path = path;
        helpProject.IsModified = false;
    }
    Boolean writeFile(HelpProjectDocument? helpProject) {
        if (helpProject is null) {
            return true;
        }
        try {
            String? path = helpProject.Path;
            if (String.IsNullOrEmpty(helpProject.Path) && !_uiMessenger.CreateSaveHelpProjectDialog(out path, helpProject.Module.Name)) {
                return true;
            }
            writeFile(helpProject, path);
        } catch (Exception ex) {
            Console.WriteLine(ex);
            throw;
        }

        return true;
    }
    void saveProjectFile(Object? obj) {
        String path;
        HelpProjectDocument helpProject = (HelpProjectDocument)_mwvm.SelectedDocument!;
        // save
        if (obj is null) {
            if (!String.IsNullOrEmpty(helpProject.Path)) {
                path = helpProject.Path;
            } else {
                if (!_uiMessenger.CreateSaveHelpProjectDialog(out path, helpProject.Module.Name)) { return; }
            }
        } else {
            // save as
            if (!_uiMessenger.CreateSaveHelpProjectDialog(out path, helpProject.Module.Name)) { return; }
        }
        try {
            writeFile(helpProject, path);
            helpProject.ErrorInfo = null;
        } catch (Exception e) {
            _uiMessenger.ShowError("Save error", e.Message);
            helpProject.ErrorInfo = e.Message;
        }
    }

    #region Import external help (MAML, CBH)

    public IAsyncCommand ImportFromMamlCommand { get; }
    public IAsyncCommand ImportFromCommentBasedHelpCommand { get; }
    Task importFromMamlHelp(Object? o, CancellationToken token) {
        var module = (PsModuleInfo)o!;

        if (_uiMessenger.CreateSaveHelpAsXmlDialog(out String? fileName, module.Name)) {
            return LoadCommandsAsync(fileName!, false);
        }

        return Task.CompletedTask;

    }
    Task importFromCommentBasedHelp(Object? o, CancellationToken token) {
        return LoadCommandsAsync(null, true);
    }
    static Boolean canImportFromHelp(Object? obj) {
        return obj != null;
    }
    
    #endregion

    async Task publishHelpFile(Object o, CancellationToken token) {
        ModuleObject module = ((HelpProjectDocument)o).Module;

        if (_uiMessenger.CreateSaveHelpAsXmlDialog(out String? fileName, module.Name)) {
            try {
                await module.PublishMamlHelpFile(fileName, _progressBar);
            } catch (Exception e) {
                _uiMessenger.ShowError("XML Write error", e.Message);
            }
        }
    }
    void publishOnline(Object? obj) {
        var dlg = new OnlinePublishProgressWindow(((HelpProjectDocument)_mwvm.SelectedDocument)!.Module);
        dlg.Show();
    }

    async Task loadModuleFromManifest(Object? o, CancellationToken token) {
        await LoadModulesCommand.ExecuteAsync(false);
        TabDocumentVM selectedDocument = _mwvm.SelectedDocument!;
        if (!_uiMessenger.CreateOpenPsManifestDialog(out String? fileName)) {
            return;
        }
        
        selectedDocument.StartSpinner(Strings.InfoModuleLoading);
        try {
            PsModuleInfo moduleInfo = await _psProcessor.GetModuleInfoFromFileAsync(fileName!);
            if (!ModuleListDocument.ModuleList.Any(x => x.Name.Equals(moduleInfo.Name))) {
                ModuleListDocument.ModuleList.Add(moduleInfo);
            }
        } catch (Exception ex) {
            _uiMessenger.ShowError("Import error", ex.Message);
            //previousTab.ErrorInfo = e.Message;
        }
        selectedDocument.StopSpinner();
    }

    // predicate
    Boolean canSave(Object? obj) {
        return _mwvm.SelectedDocument is HelpProjectDocument;
    }

    
    static Boolean canPublish(Object? obj) {
        return obj is HelpProjectDocument helpProject && helpProject.Module.Cmdlets.Count > 0;
    }
    Boolean canPublishOnline(Object obj) {
        return _mwvm.SelectedDocument is HelpProjectDocument { Module.Provider: not null };
    }

    // utility
    Boolean testSaved(HelpProjectDocument? tab) {
        if (tab is not { IsModified: true }) {
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
            case MessageBoxResult.Cancel:
                return false;
        }

        return true;
    }

    // public
    public void OpenProject(String path) {
        OpenProjectCommand.ExecuteAsync(path);
    }

    public async Task LoadCommandsAsync(String? helpPath, Boolean importFromCBH) {
        var cmd = Utils.GetCommandTypes();
        if (cmd.Count == 0) {
            _uiMessenger.ShowError("Error", Strings.E_EmptyCmds);
            return;
        }

        try {
            TabDocumentVM doc = _mwvm.SelectedDocument!;
            doc.StartSpinner(Strings.InfoCmdletsLoading);
            var moduleInfo = ((ModuleListDocument)_mwvm.SelectedDocument!).SelectedModule;
            ModuleObject? module = ModuleObject.FromPsModuleInfo(moduleInfo);
            IEnumerable<IPsCommandInfo> data;
            if (!importFromCBH && helpPath is not null) {
                module.ImportedFromHelp = true;
                data = await _psProcessor.EnumCommandsFromMamlAsync(moduleInfo, cmd, helpPath);
            } else {
                data = await _psProcessor.EnumCommandsAsync(moduleInfo, cmd, importFromCBH);
            }
            foreach (IPsCommandInfo commandInfo in data) {
                module.Cmdlets.Add(CmdletObject.FromCommandInfo(commandInfo));
            }
            var vm = new HelpProjectDocument(module);
            doc.StopSpinner();
            swapTabDocument(vm);
        } catch (Exception ex) {
            _uiMessenger.ShowError("Error while loading cmdlets", ex.Message);
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
    Boolean getOpenProjectFilePath(String? suggestedPath, out String? path) {
        if (suggestedPath is not null) {
            path = suggestedPath;
            return true;
        }

        return _uiMessenger.CreateOpenHelpProjectDialog(out path);
    }

    #endregion
}