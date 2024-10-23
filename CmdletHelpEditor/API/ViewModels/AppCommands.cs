#nullable enable
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
using PsCmdletHelpEditor.Core.Models;
using SysadminsLV.WPF.OfficeTheme.Controls;
using SysadminsLV.WPF.OfficeTheme.Toolkit;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using Unity;

namespace CmdletHelpEditor.API.ViewModels;

public class AppCommands {
    readonly MainWindowVM _mwvm;
    readonly IPsProcessor _psProcessor;
    readonly IProgressBar _progressBar;
    readonly IMsgBox _msgBox;
    Boolean alreadyRaised;

    public AppCommands(MainWindowVM parent) {
        _psProcessor = App.Container.Resolve<IPsProcessor>();
        _progressBar = App.Container.Resolve<IProgressBar>();
        _mwvm = parent;
        AddTabCommand = new RelayCommand(addTab);
        CloseTabCommand = new RelayCommand(closeTab);
        LoadModulesCommand = new AsyncCommand(loadModules, canLoadModuleList);
        LoadFromFileCommand = new AsyncCommand(loadModuleFromFile, canLoadModuleList);
        ImportFromXmlHelpCommand = new RelayCommand(importFromXmlHelp, canImportFromHelp);
        PublishHelpCommand = new AsyncCommand(publishHelpFile, canPublish);
        ImportFromCBHelpCommand = new RelayCommand(importFromCommentHelp, canImportFromHelp);
        NewCommand = new RelayCommand(newProject, canOpen);
        OpenCommand = new RelayCommand(OpenProject, canOpen);
        SaveCommand = new RelayCommand(saveProjectFile, canSave);
        CloseAppCommand = new RelayCommand<CancelEventArgs>(closeApp);
        PublishOnlineCommand = new RelayCommand(publishOnline, canPublishOnline);
        _msgBox = App.Container.Resolve<IMsgBox>();
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

    static void CopyToClipboard(Object? obj) {
        if (obj == null) { return; }
        Clipboard.SetText((String)obj);
    }
    static Boolean CanCopyClipboard(Object? obj) {
        return obj != null;
    }
    void addTab(Object? obj) {
        addTab2(obj);
        ClosableModuleItem tab = UIManager.GenerateTab();
        _mwvm.Tabs.Add(tab);
        tab.Focus();
    }
    void addTab2(Object? o) {
        var vm = new BlankDocumentVM();
        _mwvm.Documents.Add(vm);
        _mwvm.SelectedDocument = vm;
    }
    void closeTab(Object? obj) {
        closeTab2(obj);
        if (!(obj is ClosableModuleItem tab)) { return; }
        if (_mwvm.SelectedTab.IsSaved) {
            _mwvm.Tabs.Remove(tab);
        } else {
            if (testSaved(tab)) {
                _mwvm.Tabs.Remove(tab);
            }
        }
    }
    void closeTab2(Object? o) {
        if (_mwvm.SelectedDocument is not null) {
            _mwvm.Documents.Remove(_mwvm.SelectedDocument);
        }
    }
    // toolbar/menu commands
    void newProject(Object? obj) {
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
    void newProject2(Object? o) {

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
        _mwvm.SelectedTab.Header = (new FileInfo(path)).Name;
        try {
            FileProcessor.SaveProjectFile(_mwvm.SelectedTab, path);
        }
        catch (Exception e) {
            _msgBox.ShowError("Save error", e.Message);
            _mwvm.SelectedTab.ErrorInfo = e.Message;
        }
    }
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
    void importFromXmlHelp(Object? obj) {
        var module = (ModuleObject)obj;
        var dlg = new OpenFileDialog {
            FileName = module.Name + ".Help.xml",
            DefaultExt = ".xml",
            Filter = "PowerShell Help Xml files (.xml)|*.xml"
        };
        Boolean? result = dlg.ShowDialog();
        if (result == true) {
        } else { return; }
        LoadCmdlets(dlg.FileName, false);
    }
    void importFromCommentHelp(Object? obj) {
        LoadCmdlets(null, true);
    }
    async Task publishHelpFile(Object o, CancellationToken token) {
        ModuleObject module = ((ClosableModuleItem)o).Module;
        var dlg = new SaveFileDialog {
            FileName = _mwvm.SelectedTab.Module.Name + ".Help.xml",
            DefaultExt = ".xml",
            Filter = "PowerShell Help Xml files (.xml)|*.xml"
        };
        Boolean? result = dlg.ShowDialog();
        if (result == true) {
            try {
                await module.PublishHelpFile(dlg.FileName, _progressBar);
            }
            catch (Exception e) {
                _msgBox.ShowError("XML Write error", e.Message);
            }
        }
    }
    void publishOnline(Object? obj) {
        var dlg = new OnlinePublishProgressWindow(_mwvm.SelectedTab.Module);
        dlg.Show();
    }

    async Task loadModules(Object? o, CancellationToken token) {
        await loadModules2(o, token);
        // method call from ICommand is allowed only when module selector is active
        // so skip checks.
        ClosableModuleItem previousTab = _mwvm.SelectedTab;
        UIManager.ShowBusy(previousTab, Strings.InfoModuleListLoading);
        _mwvm.Modules.Clear();
        try {
            await _psProcessor.EnumModulesAsync(o == null);

            foreach (ModuleObject item in _psProcessor.ModuleList) {
                _mwvm.Modules.Add(item);
            }
        }
        catch (Exception e) {
            _msgBox.ShowError("Error", e.Message);
            previousTab.ErrorInfo = e.Message;
        }
        UIManager.ShowModuleList(previousTab);
    }
    async Task loadModules2(Object? o, CancellationToken token) {
        var vm = new ModuleListDocument() {
            IsBusy = true,
            MWVM = _mwvm
        };
        swapTabDocument(vm);
        IEnumerable<PsModuleInfo> modules = await _psProcessor.EnumModulesAsync(o == null);
        foreach (PsModuleInfo moduleInfo in modules) {
            vm.ModuleList.Add(moduleInfo);
        }

        vm.IsBusy = false;
    }
    async Task loadModuleFromFile(Object? o, CancellationToken token) {
        // method call from ICommand is allowed only when module selector is active
        // so skip checks.
        ClosableModuleItem previousTab = _mwvm.SelectedTab;
        var dlg = new OpenFileDialog {
            DefaultExt = ".psm1",
            Filter = "PowerShell module files (*.psm1, *.psd1)|*.psm1;*.psd1"
        };
        Boolean? result = dlg.ShowDialog();
        if (result != true) { return; }
        UIManager.ShowBusy(previousTab, Strings.InfoModuleLoading);
        try {
            ModuleObject module = await _psProcessor.GetModuleFromFileAsync(dlg.FileName);
            if (module != null && !_mwvm.Modules.Contains(module)) {
                _mwvm.Modules.Add(module);
                module.ModulePath = dlg.FileName;
            }
        }
        catch (Exception e) {
            _msgBox.ShowError("Import error", e.Message);
            previousTab.ErrorInfo = e.Message;
        }
        UIManager.ShowModuleList(previousTab);
    }
    async Task loadCmdletsForProject(ClosableModuleItem tab) {
        String cmd = Utils.GetCommandTypes();
        if (String.IsNullOrEmpty(cmd)) {
            _msgBox.ShowError("Error", Strings.E_EmptyCmds);
            return;
        }
        if (FileProcessor.FindModule(tab.Module.Name)) {
            tab.Module.ModulePath = null;
        }
        tab.ErrorInfo = null;
        tab.EditorContext.CurrentCmdlet = null;
        var nativeCmdlets = new List<CmdletObject>();
        try {
            IEnumerable<CmdletObject> data = await _psProcessor.EnumCmdletsAsync(tab.Module, cmd, false);
            nativeCmdlets.AddRange(data);
            tab.Module.CompareCmdlets(nativeCmdlets);
        }
        catch (Exception e) {
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
        finally {
            UIManager.ShowEditor(tab);
        }
    }

    // predicate
    Boolean canOpen(Object? obj) {
        return _mwvm.SelectedTab == null ||
               ((Grid)_mwvm.SelectedTab.Content).Children.Count == 0 ||
               !(((Grid)_mwvm.SelectedTab.Content).Children[0] is LoadingSpinner);
    }
    Boolean canSave(Object? obj) {
        return _mwvm.SelectedTab is { Module.UpgradeRequired: false };
    }
    Boolean canLoadModuleList(Object? obj) {
        return true;
        return ((Grid)_mwvm.SelectedTab?.Content)?.Children[0] is ModuleSelectorControl;
    }
    static Boolean canImportFromHelp(Object? obj) {
        return obj != null;
    }
    static Boolean canPublish(Object? obj) {
        //Object[] param = (Object[])obj;
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
    Boolean getSaveFileName(out String? path) {
        var dlg = new SaveFileDialog {
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
    public void OpenProject(Object? obj) {
        String fileName;
        var dlg = new OpenFileDialog {
            DefaultExt = ".pshproj",
            Filter = "PowerShell Help Project file (.pshproj)|*.pshproj"
        };
        if (obj == null) {
            Boolean? result = dlg.ShowDialog();
            if (result != true) {
                return;
            }
            fileName = dlg.FileName;
        } else {
            fileName = (String)obj;
        }
        addTab(null);
        ClosableModuleItem? tab = _mwvm.SelectedTab;
        UIManager.ShowBusy(tab, Strings.InfoCmdletsLoading);
        try {
            ModuleObject module = FileProcessor.ReadProjectFile(fileName);
            tab!.Module = module;
            loadCmdletsForProject(tab);
        }
        catch (Exception e) {
            _msgBox.ShowError("Read error", e.Message);
        }
    }
    public async void LoadCmdlets(Object? helpPath, Boolean importCBH) {
        ClosableModuleItem previousTab = _mwvm.SelectedTab;
        UIElement previousElement = ((Grid)previousTab.Content).Children[0];
        String cmd = Utils.GetCommandTypes();
        if (String.IsNullOrEmpty(cmd)) {
            _msgBox.ShowError("Error", Strings.E_EmptyCmds);
            return;
        }
        UIManager.ShowBusy(previousTab, Strings.InfoCmdletsLoading);
        try {
            IEnumerable<CmdletObject> data = await _psProcessor.EnumCmdletsAsync(_mwvm.SelectedModule, cmd, importCBH);
            _mwvm.SelectedModule.Cmdlets.Clear();
            foreach (CmdletObject item in data) {
                _mwvm.SelectedModule.Cmdlets.Add(item);
            }
            if (helpPath != null) {
                _mwvm.SelectedModule.ImportedFromHelp = true;
                try {
                    XmlProcessor.ImportFromXml((String)helpPath, _mwvm.SelectedModule);
                }
                catch (Exception ex) {
                    _msgBox.ShowError("Error", ex.Message);
                }
            }
            previousTab.Module = _mwvm.SelectedModule;
            _mwvm.SelectedModule = null;
            UIManager.ShowEditor(previousTab);
        }
        catch (Exception e) {
            _msgBox.ShowError("Error while loading cmdlets", e.Message);
            _mwvm.SelectedTab.ErrorInfo = e.Message;
            UIManager.RestoreControl(previousTab, previousElement);
        }
    }
    public async Task LoadCmdletsAsync2(String? helpPath, Boolean importFromCBH) {
        var vm = new HelpProjectDocument();
        String cmd = Utils.GetCommandTypes();
        if (String.IsNullOrEmpty(cmd)) {
            _msgBox.ShowError("Error", Strings.E_EmptyCmds);
            return;
        }

        try {
            var moduleInfo = ((ModuleListDocument)_mwvm.SelectedDocument).SelectedModule;
            var module = new ModuleObject(moduleInfo);
            IEnumerable<CmdletObject> data = await _psProcessor.EnumCmdletsAsync(module, cmd, importFromCBH);
            foreach (CmdletObject item in data) {
                module.Cmdlets.Add(item);
            }
            if (helpPath != null) {
                module.ImportedFromHelp = true;
                try {
                    XmlProcessor.ImportFromXml(helpPath, module);
                } catch (Exception ex) {
                    _msgBox.ShowError("Error", ex.Message);
                }
            }
            vm.Module = module;
            vm.EditorContext = new EditorVM(module);
            swapTabDocument(vm);
        }
        catch (Exception ex) {
            Console.WriteLine(ex);
            throw;
        }
    }

    void swapTabDocument(TabDocumentVM newDocument) {
        Int32 index = _mwvm.Documents.IndexOf(_mwvm.SelectedDocument);
        _mwvm.Documents[index] = newDocument;
        _mwvm.SelectedDocument = newDocument;
    }
}