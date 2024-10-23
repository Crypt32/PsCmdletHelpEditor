using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.Properties;
using PsCmdletHelpEditor.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace CmdletHelpEditor.API.Tools;
class PowerShellProcessor : IPsProcessor {
    static readonly IList<String> _excludedModules = new List<String> {
        "Microsoft.PowerShell.Core",
        "Microsoft.PowerShell.Host",
        "Microsoft.PowerShell.Management",
        "Microsoft.PowerShell.Security",
        "Microsoft.PowerShell.Utility",
        "Microsoft.WSMan.Management"
    };
    static readonly List<PsModuleInfo> _moduleList = [];

    public Int32? PsVersion { get; set; }
    public IList<ModuleObject> ModuleList { get; } = new List<ModuleObject>();

    public Task<Int32?> GetPsVersionAsync() {
        return Task.Factory.StartNew(getPsVersion);
    }
    Int32? getPsVersion() {
        PowerShell ps = PowerShell.Create();
        ps.AddScript("$PSVersionTable.PSVersion.Major");
        PsVersion = (Int32?)ps.Invoke()[0].BaseObject;
        ps.Dispose();
        if (PsVersion >= 3) {
            Settings.Default.WorkflowEnabled = true;
        }
        if (PsVersion >= 4) {
            Settings.Default.ConfigurationEnabled = true;
        }

        return PsVersion;
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
    public IEnumerable<PsModuleInfo> EnumModules(Boolean force) {
        if (!force) {
            return _moduleList;
        }

        ModuleList.Clear();
        using PowerShell ps = PowerShell.Create();
        ps.AddCommand("Get-Module").AddParameter("ListAvailable");
        List<PSObject> modules = ps.Invoke()
            .Where(x => !_excludedModules.Contains(x.Members["Name"].Value.ToString()))
            .ToList();
        foreach (PSObject psModule in modules) {
            addModule(psModule);
            ModuleList.Add(
                new ModuleObject {
                    Name = (String)psModule.Members["Name"].Value,
                    ModuleType = (ModuleType)psModule.Members["ModuleType"].Value,
                    Version = psModule.Members["Version"].Value.ToString(),
                    Description = (String)psModule.Members["Description"].Value,
                    ModuleClass = "Module",
                    HasManifest = new FileInfo((String)psModule.Members["Path"].Value).Extension.ToLower() == ".psd1"
                });
        }

        ps.Commands.Clear();
        ps.AddCommand("Get-PSSnapin").AddParameter("Registered");
        modules = ps.Invoke().ToList();
        ps.Commands.Clear();
        ps.AddCommand("Get-PSSnapin");
        modules.AddRange(ps.Invoke().Where(x => !_excludedModules.Contains(x.Members["Name"].Value.ToString())).ToList());
        foreach (PSObject psSnapIn in modules) {
            addPsSnapIn(psSnapIn);
            ModuleList.Add(
                new ModuleObject {
                    Name = (String)psSnapIn.Members["Name"].Value,
                    ModuleType = ModuleType.Binary,
                    Version = psSnapIn.Members["Version"].Value.ToString(),
                    Description = (String)psSnapIn.Members["Description"].Value,
                    ModuleClass = "Snapin"
                });
        }

        return _moduleList;
    }

    /// <param name="force">Forces module list reload. Otherwise, cached list is returned.</param>
    /// <returns></returns>
    public Task<IEnumerable<PsModuleInfo>> EnumModulesAsync(Boolean force) {
        return Task.Factory.StartNew(() => EnumModules(force));
    }
    static IEnumerable<CmdletObject> enumCmdlets(ModuleObject module, String commandTypes, Boolean fromCBH) {
        module.IsOffline = false;
        using PowerShell ps = PowerShell.Create();
        ps.AddScript(module.GetInvocationString(commandTypes));
        try {
            return ps.Invoke()
                .Where(x => (CommandTypes)x.Members["CommandType"].Value != CommandTypes.Alias)
                .Select(x => new CmdletObject(x, fromCBH));
        }
        catch {
            module.IsOffline = true;
            throw;
        }
    }
    public Task<IEnumerable<CmdletObject>> EnumCmdletsAsync(ModuleObject module, String commandTypes, Boolean fromCBH) {
        return Task.Factory.StartNew(() => enumCmdlets(module, commandTypes, fromCBH));
    }
    public Task<ModuleObject> GetModuleFromFileAsync(String path) {
        return Task<ModuleObject>.Factory.StartNew(() => getModuleFromFile(path));
    }
    static ModuleObject getModuleFromFile(String path) {
        using PowerShell ps = PowerShell.Create();
        ps.AddCommand("Import-Module").AddParameter("Name", path).AddParameter("PassThru");
        List<PSObject> psModule = ps.Invoke().ToList();
        return new ModuleObject {
            Name = (String)psModule[0].Members["Name"].Value,
            ModuleType = (ModuleType)psModule[0].Members["ModuleType"].Value,
            Version = psModule[0].Members["Version"].Value.ToString(),
            Description = (String)psModule[0].Members["Description"].Value,
            ModuleClass = "External",
            ModulePath = path
        };
    }
}
