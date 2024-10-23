using System;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models;

public class PsModuleInfo {
    public String Name { get; set; }
    public ModuleType ModuleType { get; set; }
    public String Version { get; set; }
    public String Description { get; set; }
    public String ModuleClass { get; set; }
    public Boolean HasManifest { get; set; }
}