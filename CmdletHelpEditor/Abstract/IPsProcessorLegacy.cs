using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CmdletHelpEditor.API.Models;
using PsCmdletHelpEditor.Core.Models;

namespace CmdletHelpEditor.Abstract;
[Obsolete]
public interface IPsProcessorLegacy {

    IList<ModuleObject> ModuleList { get; }
    Task<IEnumerable<CmdletObject>> EnumCmdletsAsync(IModuleInfo module, String commandTypes, Boolean fromCBH);
    [Obsolete]
    Task<ModuleObject> GetModuleFromFileAsync(String path);
}