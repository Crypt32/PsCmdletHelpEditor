using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models;

public interface IPsModuleProject {
    String Description { get; }
    String ExtraFooter { get; }
    String ExtraHeader { get; }
    Int32? FetchPostCount { get; }
    Double FormatVersion { get; }
    Boolean HasManifest { get; }
    String ModuleClass { get; }
    String ModulePath { get; }
    ModuleType ModuleType { get; }
    String Name { get; }
    Boolean OverridePostCount { get; }
    Boolean UseSupports { get; }
    String Version { get; }
    String ProjectPath { get; set; }

    IReadOnlyList<IPsCommandInfo> GetCmdlets();
    IXmlRpcProviderInformation? GetXmlRpcProviderInfo();
}