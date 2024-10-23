using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PsCmdletHelpEditor.Core.Models;

namespace PsCmdletHelpEditor.Core.Services;

public interface IPowerShellProcessor {
    Task<Int32?> GetPsVersionAsync();
    Task<IEnumerable<PsModuleInfo>> EnumModulesAsync(Boolean force);
    //Task<IEnumerable<CmdletObject>> EnumCmdletsAsync(ModuleObject module, String cmds, Boolean fromCBH);
    //Task<ModuleObject> GetModuleFromFileAsync(String path);
}