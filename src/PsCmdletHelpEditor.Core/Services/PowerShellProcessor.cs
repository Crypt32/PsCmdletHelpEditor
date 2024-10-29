using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Models.PowerShellNative;

namespace PsCmdletHelpEditor.Core.Services;

public class PowerShellProcessor : IPowerShellProcessor {
    static readonly List<String> _excludedModules = [
        "Microsoft.PowerShell.Core",
        "Microsoft.PowerShell.Host",
        "Microsoft.PowerShell.Management",
        "Microsoft.PowerShell.Security",
        "Microsoft.PowerShell.Utility",
        "Microsoft.WSMan.Management"
    ];
    static readonly List<PsModuleInfo> _moduleList = [];

    public Int32? PsVersion { get; private set; }

    public Task<Int32?> GetPsVersionAsync() {
        return Task.Factory.StartNew(GetPsVersion);
    }
    public Int32? GetPsVersion() {
        using PowerShell ps = PowerShell.Create();
        ps.AddScript("$PSVersionTable.PSVersion.Major");
        PsVersion = (Int32?)ps.Invoke()[0].BaseObject;

        return PsVersion;
    }
    public Task<IEnumerable<PsModuleInfo>> EnumModulesAsync(Boolean force) {
        return Task.Factory.StartNew(() => EnumModules(force));
    }
    public IEnumerable<PsModuleInfo> EnumModules(Boolean force) {
        if (!force || _moduleList.Count > 0) {
            return _moduleList;
        }

        _moduleList.Clear();
        using PowerShell ps = PowerShell.Create();
        ps.AddCommand("Get-Module").AddParameter("ListAvailable");
        List<PSObject> modules = ps.Invoke()
            .Where(x => !_excludedModules.Contains(x.Members["Name"].Value.ToString()))
            .ToList();
        foreach (PSObject psModule in modules) {
            addModule(psModule);
        }

        ps.Commands.Clear();
        ps.AddCommand("Get-PSSnapin").AddParameter("Registered");
        modules = ps.Invoke().ToList();
        ps.Commands.Clear();
        ps.AddCommand("Get-PSSnapin");
        modules.AddRange(ps.Invoke().Where(x => !_excludedModules.Contains(x.Members["Name"].Value.ToString())).ToList());
        foreach (PSObject psSnapIn in modules) {
            addPsSnapIn(psSnapIn);
        }

        return _moduleList;
    }
    static void addModule(PSObject psObject) {
        _moduleList.Add(new PsModuleInfo {
            Name = psObject.Members["Name"].Value.ToString(),
            ModuleType = (ModuleType)psObject.Members["ModuleType"].Value,
            Version = psObject.Members["Version"].Value.ToString(),
            Description = psObject.Members["Description"].Value.ToString(),
            ModuleClass = "Module",
            HasManifest = new FileInfo((String)psObject.Members["Path"].Value).Extension.ToLower() == ".psd1"
        });
    }
    static void addPsSnapIn(PSObject psObject) {
        _moduleList.Add(new PsModuleInfo {
            Name = psObject.Members["Name"].Value.ToString(),
            ModuleType = ModuleType.Binary,
            Version = psObject.Members["Version"].Value.ToString(),
            Description = psObject.Members["Description"].Value.ToString(),
            ModuleClass = "Snapin"
        });
    }
    public Task<PsModuleInfo> GetModuleInfoFromFileAsync(String path) {
        return Task.Factory.StartNew(() => GetModuleInfoFromFile(path));
    }
    public PsModuleInfo GetModuleInfoFromFile(String path) {
        using PowerShell ps = PowerShell.Create();
        ps.AddCommand("Import-Module").AddParameter("Name", path).AddParameter("PassThru");
        List<PSObject> psModule = ps.Invoke().ToList();
        return new PsModuleInfo {
            Name = (String)psModule[0].Members["Name"].Value,
            ModuleType = (ModuleType)psModule[0].Members["ModuleType"].Value,
            Version = psModule[0].Members["Version"].Value.ToString(),
            Description = (String)psModule[0].Members["Description"].Value,
            ModuleClass = "External",
            ModulePath = path
        };
    }
    
    public Task<IEnumerable<IPsCommandInfo>> EnumCommandsAsync(PsModuleInfo moduleInfo, IEnumerable<String> commandTypes, Boolean includeCBH = false) {
        return Task.Factory.StartNew(() => EnumCommands(moduleInfo, commandTypes, includeCBH));
    }
    public IEnumerable<IPsCommandInfo> EnumCommands(PsModuleInfo moduleInfo, IEnumerable<String> commandTypes, Boolean includeCBH = false) {
        moduleInfo.IsOffline = false;
        using PowerShell ps = PowerShell.Create();
        ps.AddScript(moduleInfo.GetInvocationString(commandTypes));
        try {
            return ps.Invoke()
                .Where(x => (CommandTypes)x.Members["CommandType"].Value != CommandTypes.Alias)
                .Select(x => PsCommandInfo.FromCommandInfo(x, includeCBH));
        } catch {
            moduleInfo.IsOffline = true;
            throw;
        }
    }
    public Boolean TestModuleExist(String moduleName) {
        String? modulePaths = Environment.GetEnvironmentVariable("PSModulePath", EnvironmentVariableTarget.Process);
        if (String.IsNullOrEmpty(modulePaths)) {
            return false;
        }
        return modulePaths
            .Split([';'], StringSplitOptions.RemoveEmptyEntries)
            .Select(path => new DirectoryInfo(path))
            .Where(x => x.Exists)
            .Any(dir => dir.EnumerateDirectories(moduleName).Any());
    }
}