using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.Properties;
using PsCmdletHelpEditor.Core.Models;

namespace CmdletHelpEditor.API.Tools;
[Obsolete]
class PowerShellProcessorLegacy : IPsProcessorLegacy {
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

    static IEnumerable<CmdletObject> enumCmdlets(IModuleInfo module, String commandTypes, Boolean fromCBH) {
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
    public Task<IEnumerable<CmdletObject>> EnumCmdletsAsync(IModuleInfo module, String commandTypes, Boolean fromCBH) {
        return Task.Factory.StartNew(() => enumCmdlets(module, commandTypes, fromCBH));
    }
    public Task<ModuleObject> GetModuleFromFileAsync(String path) {
        return Task.Factory.StartNew(() => getModuleFromFile(path));
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
