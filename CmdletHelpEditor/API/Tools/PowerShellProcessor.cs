using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.Properties;

namespace CmdletHelpEditor.API.Tools {
    static class PowerShellProcessor {
        public static Int32? PsVersion { get; set; }
        static List<ModuleObject> ModuleList { get; set; }

        static String getInvocationString(ModuleObject module, String cmds) {
            Object[] args = new Object[5];
            args[1] = module.Name;
            args[4] = cmds;
            if (module.ModuleClass == "Module" || module.ModuleClass == "External") {
                args[0] = "Import-Module";
                args[2] = String.IsNullOrEmpty(module.ModulePath)
                    ? module.Name
                    : module.ModulePath;
                args[3] = module.ModuleClass == "External" || !module.HasManifest
                    ? null
                    : " -RequiredVersion " + module.Version;
                return String.Format(Resources.ipmoTemplate, args);
            }
            args[0] = "Add-PSSnapin";
            args[2] = args[3] = null;
            return String.Format(Resources.ipmoTemplate, args);
        }
        /// <param name="sourceCmdlet">active cmdlet from module</param>
        /// <param name="destinationCmdlet">saved cmdlet in project file</param>
        static void CopyParameters(CmdletObject sourceCmdlet, CmdletObject destinationCmdlet) {
            List<String> processed = new List<String>();
            // process saved parameters
            for (Int32 index = 0; index < destinationCmdlet.Parameters.Count; index++) {
                Int32 sourceIndex = sourceCmdlet.Parameters.IndexOf(destinationCmdlet.Parameters[index]);
                if (sourceIndex >= 0) {
                    // copy user input to source cmdlet
                    sourceCmdlet.Parameters[sourceIndex].Description = destinationCmdlet.Parameters[index].Description;
                    sourceCmdlet.Parameters[sourceIndex].Globbing = destinationCmdlet.Parameters[index].Globbing;
                    sourceCmdlet.Parameters[sourceIndex].DefaultValue = destinationCmdlet.Parameters[index].DefaultValue;
                    // replace parameter from source to destination cmdlet
                    destinationCmdlet.Parameters[index] = sourceCmdlet.Parameters[sourceIndex];
                    processed.Add(destinationCmdlet.Parameters[index].Name);
                } else {
                    // saved cmdlet contains orphaned parameter
                    destinationCmdlet.Parameters[index].Status = ItemStatus.Missing;
                }
            }
            // process active non-processed parameters. They are new parameters
            foreach (ParameterDescription param in sourceCmdlet.Parameters.Where(param => !processed.Contains(param.Name))) {
                destinationCmdlet.Parameters.Add(param);
            }
        }

        public static Task<Int32?> GetPsVersion() {
            return Task.Factory.StartNew(() => {
                PowerShell ps = PowerShell.Create();
                ps.AddScript("$PSVersionTable.PSVersion.Major");
                PsVersion = (Int32?) ps.Invoke()[0].BaseObject;
                ps.Dispose();
                if (PsVersion >= 3) { Settings.Default.WorkflowEnabled = true; }
                if (PsVersion >= 4) { Settings.Default.ConfigurationEnabled = true; }
                return PsVersion;
            });
        }
        /// <param name="force">Forces module list reload. Otherwise cached list is returned.</param>
        /// <returns></returns>
        public static Task<IEnumerable<ModuleObject>> EnumModules(Boolean force) {
            return Task<IEnumerable<ModuleObject>>.Factory.StartNew(() => {
                if (ModuleList != null && !force) { return ModuleList; }
                PowerShell ps = PowerShell.Create();
                try {
                    ps.AddCommand("Get-Module").AddParameter("ListAvailable");
                    List<PSObject> modules = ps.Invoke().ToList();

                    ModuleList = modules.Select(item => new ModuleObject {
                        Name = (String)item.Members["Name"].Value,
                        ModuleType = (ModuleType)item.Members["ModuleType"].Value,
                        Version = item.Members["Version"].Value.ToString(),
                        Description = (String)item.Members["Description"].Value,
                        ModuleClass = "Module",
                        HasManifest = (new FileInfo((String)item.Members["Path"].Value)).Extension.ToLower() == ".psd1",
                    }).ToList();

                    ps.Commands.Clear();
                    ps.AddCommand("Get-PSSnapin").AddParameter("Registered");
                    modules = ps.Invoke().ToList();
                    ps.Commands.Clear();
                    ps.AddCommand("Get-PSSnapin");
                    modules.AddRange(ps.Invoke().ToList());

                    ModuleList.AddRange(modules.Select(item => new ModuleObject {
                        Name = (String)item.Members["Name"].Value,
                        ModuleType = ModuleType.Binary,
                        Version = item.Members["Version"].Value.ToString(),
                        Description = (String)item.Members["Description"].Value,
                        ModuleClass = "Snapin"
                    }).ToList());
                }
                finally {
                    ps.Dispose();
                }
                List<String> exclude = new List<String> {
                    "Microsoft.PowerShell.Core",
                    "Microsoft.PowerShell.Host",
                    "Microsoft.PowerShell.Management",
                    "Microsoft.PowerShell.Security",
                    "Microsoft.PowerShell.Utility",
                    "Microsoft.WSMan.Management"
                };
                return ModuleList.Where(x => !exclude.Contains(x.Name)).ToList();
            });
        }
        public static Task<IEnumerable<CmdletObject>> EnumCmdlets(ModuleObject module, String cmds, Boolean fromCBH) {
            return Task<IEnumerable<CmdletObject>>.Factory.StartNew(() => {
                Boolean fired = false;
                PowerShell ps = PowerShell.Create();
                ps.AddScript(getInvocationString(module, cmds));
                try {
                    return (
                        from item in ps.Invoke().ToList()
                        where (CommandTypes)item.Members["CommandType"].Value != CommandTypes.Alias
                        select new CmdletObject(item, fromCBH)).ToList();
                } catch {
                    fired = true;
                    throw;
                }
                finally {
                    ps.Dispose();
                    module.IsOffline = fired;
                }
            });
        }
        public static Task<ModuleObject> GetModuleFromFile(String path) {
            return Task<ModuleObject>.Factory.StartNew(() => {
                    PowerShell ps = PowerShell.Create();
                    ps.AddCommand("Import-Module").AddParameter("Name", path).AddParameter("PassThru");
                try {
                    List<PSObject> psmodule = ps.Invoke().ToList();
                    ModuleObject retValue = new ModuleObject {
                        Name = (String)psmodule[0].Members["Name"].Value,
                        ModuleType = (ModuleType)psmodule[0].Members["ModuleType"].Value,
                        Version = psmodule[0].Members["Version"].Value.ToString(),
                        Description = (String)psmodule[0].Members["Description"].Value,
                        ModuleClass = "External",
                        ModulePath = path
                    };
                    return retValue;
                } finally {
                    ps.Dispose();
                }
            });
        }
        /// <param name="module">Module imported from project</param>
        /// <param name="cmdlets">active cmdlets from online module</param>
        public static void CompareCmdlets(ModuleObject module, List<CmdletObject> cmdlets) {
            if (module.Cmdlets.Count == 0) {
                module.Cmdlets = new ObservableCollection<CmdletObject>(cmdlets);
                return;
            }
            List<String> processed = new List<String>();
            // process saved cmdlets
            foreach (CmdletObject cmdlet in module.Cmdlets) {
                Int32 activeCmdletIndex = cmdlets.IndexOf(cmdlet);
                if (activeCmdletIndex >= 0) {
                    // update syntax, parametersets and parameter information from active cmdlet to project
                    cmdlet.Syntax = cmdlets[activeCmdletIndex].Syntax;
                    cmdlet.ParameterSets = cmdlets[activeCmdletIndex].ParameterSets;
                    cmdlet.UpdateParamSets();
                    CopyParameters(cmdlets[activeCmdletIndex], cmdlet);
                    processed.Add(cmdlet.Name);
                } else {
                    // saved project contains orphaned cmdlet
                    cmdlet.GeneralHelp.Status = ItemStatus.Missing;
                }
            }
            // add new cmdlets to the project if any
            foreach (CmdletObject cmdlet in cmdlets.Where(cmdlet => !processed.Contains(cmdlet.Name))) {
                module.Cmdlets.Add(cmdlet);
            }
        }
    }
}
