using PsCmdletHelpEditor.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

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

    Int32? psVersion;

    public Task<Int32?> GetPsVersionAsync() {
        return Task.Factory.StartNew(getPsVersion);
    }
    Int32? getPsVersion() {
        PowerShell ps = PowerShell.Create();
        ps.AddScript("$PSVersionTable.PSVersion.Major");
        psVersion = (Int32?)ps.Invoke()[0].BaseObject;
        ps.Dispose();

        return psVersion;
    }
    public Task<IEnumerable<PsModuleInfo>> EnumModulesAsync(Boolean force) {
        return Task.Factory.StartNew(() => enumModulesAsync(force));
    }
    IEnumerable<PsModuleInfo> enumModulesAsync(Boolean force) {
        if (!force) {
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
}