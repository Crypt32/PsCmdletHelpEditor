using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CmdletHelpEditor.API.Models;

namespace CmdletHelpEditor.Abstract {
    public interface IPsProcessor {
        Int32? PsVersion { get; }
        IList<ModuleObject> ModuleList { get; }

        Task<Int32?> GetPsVersion();
        Task EnumModules(Boolean force);
        Task<IEnumerable<CmdletObject>> EnumCmdlets(ModuleObject module, String cmds, Boolean fromCBH);
        Task<ModuleObject> GetModuleFromFile(String path);
    }
}