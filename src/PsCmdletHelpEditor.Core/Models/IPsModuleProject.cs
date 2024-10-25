using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models;

public interface IPsModuleProject {
    String Description { get; set; }
    String ExtraFooter { get; set; }
    String ExtraHeader { get; set; }
    Int32? FetchPostCount { get; set; }
    Double FormatVersion { get; set; }
    Boolean HasManifest { get; set; }
    String ModuleClass { get; set; }
    String ModulePath { get; set; }
    ModuleType ModuleType { get; set; }
    String Name { get; set; }
    Boolean OverridePostCount { get; set; }
    Boolean UseSupports { get; set; }
    String Version { get; set; }
    String ProjectPath { get; }

    IReadOnlyList<IPsCommandInfo> GetCmdlets();
    IXmlRpcProviderInformation? GetXmlRpcProviderInfo();
}