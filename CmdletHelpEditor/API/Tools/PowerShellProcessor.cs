using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.Properties;

namespace CmdletHelpEditor.API.Tools {
    class PowerShellProcessor : IPsProcessor {
        static readonly IList<String> _excludedModules = new List<String> {
            "Microsoft.PowerShell.Core",
            "Microsoft.PowerShell.Host",
            "Microsoft.PowerShell.Management",
            "Microsoft.PowerShell.Security",
            "Microsoft.PowerShell.Utility",
            "Microsoft.WSMan.Management"
        };

        public Int32? PsVersion { get; set; }
        public IList<ModuleObject> ModuleList { get; } = new List<ModuleObject>();

        public Task<Int32?> GetPsVersion() {
            return Task.Factory.StartNew(() => {
                PowerShell ps = PowerShell.Create();
                ps.AddScript("$PSVersionTable.PSVersion.Major");
                PsVersion = (Int32?)ps.Invoke()[0].BaseObject;
                ps.Dispose();
                if (PsVersion >= 3) { Settings.Default.WorkflowEnabled = true; }
                if (PsVersion >= 4) { Settings.Default.ConfigurationEnabled = true; }
                return PsVersion;
            });
        }
        /// <param name="force">Forces module list reload. Otherwise cached list is returned.</param>
        /// <returns></returns>
        public Task EnumModules(Boolean force) {
            return Task.Factory.StartNew(() => {
                if (!force) {
                    return;
                }

                ModuleList.Clear();
                using (PowerShell ps = PowerShell.Create()) {
                    ps.AddCommand("Get-Module").AddParameter("ListAvailable");
                    List<PSObject> modules = ps.Invoke()
                        .Where(x => _excludedModules.Contains(x.Members["Name"].Value.ToString()))
                        .ToList();

                    modules.ForEach(x => ModuleList.Add(
                                        new ModuleObject {
                                            Name = (String)x.Members["Name"].Value,
                                            ModuleType = (ModuleType)x.Members["ModuleType"].Value,
                                            Version = x.Members["Version"].Value.ToString(),
                                            Description = (String)x.Members["Description"].Value,
                                            ModuleClass = "Module",
                                            HasManifest = new FileInfo((String)x.Members["Path"].Value).Extension.ToLower() == ".psd1"
                                        }));

                    ps.Commands.Clear();
                    ps.AddCommand("Get-PSSnapin").AddParameter("Registered");
                    modules = ps.Invoke().ToList();
                    ps.Commands.Clear();
                    ps.AddCommand("Get-PSSnapin");
                    modules.AddRange(ps.Invoke().ToList());

                    modules.ForEach(x => ModuleList.Add(
                                        new ModuleObject {
                                            Name = (String)x.Members["Name"].Value,
                                            ModuleType = ModuleType.Binary,
                                            Version = x.Members["Version"].Value.ToString(),
                                            Description = (String)x.Members["Description"].Value,
                                            ModuleClass = "Snapin"
                                        }));
                }
            });
        }
        public Task<IEnumerable<CmdletObject>> EnumCmdlets(ModuleObject module, String commandTypes, Boolean fromCBH) {
            return Task<IEnumerable<CmdletObject>>.Factory.StartNew(() => {
                module.IsOffline = false;
                using (PowerShell ps = PowerShell.Create()) {
                    ps.AddScript(module.GetInvocationString(commandTypes));
                    try {
                        return ps.Invoke()
                            .Where(x => (CommandTypes)x.Members["CommandType"].Value != CommandTypes.Alias)
                            .Select(x => new CmdletObject(x, fromCBH));
                    } catch {
                        module.IsOffline = true;
                        throw;
                    }
                }
            });
        }
        public Task<ModuleObject> GetModuleFromFile(String path) {
            return Task<ModuleObject>.Factory.StartNew(() => {
                using (PowerShell ps = PowerShell.Create()) {
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
            });
        }
    }
}
