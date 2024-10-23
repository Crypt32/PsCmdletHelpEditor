using CmdletHelpEditor.API.Models;
using PsCmdletHelpEditor.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CmdletHelpEditor.Abstract;
public interface IPsProcessor {
    Int32? PsVersion { get; }
    IList<ModuleObject> ModuleList { get; }

    Task<Int32?> GetPsVersionAsync();
    Task<IEnumerable<PsModuleInfo>> EnumModulesAsync(Boolean force);
    Task<IEnumerable<CmdletObject>> EnumCmdletsAsync(ModuleObject module, String cmds, Boolean fromCBH);
    Task<ModuleObject> GetModuleFromFileAsync(String path);
}