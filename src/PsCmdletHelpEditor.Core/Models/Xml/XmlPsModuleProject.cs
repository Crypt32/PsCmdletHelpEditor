using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Xml.Serialization;

namespace PsCmdletHelpEditor.Core.Models.Xml;

[XmlRoot("ModuleObject")]
[XmlInclude(typeof(XmlPsCommand))]
public class XmlPsModuleProject : IPsModuleProject {
    [XmlAttribute("fVersion")]
    public Double FormatVersion { get; set; }
    public String Name { get; set; }
    [XmlAttribute("type")]
    public ModuleType ModuleType { get; set; }
    public String Version { get; set; }
    public String Description { get; set; }
    [XmlAttribute("mclass")]
    public String ModuleClass { get; set; }
    [XmlIgnore]
    public String ModulePath { get; set; }
    [XmlAttribute("useSupports")]
    public Boolean UseSupports { get; set; }
    public Boolean HasManifest { get; set; }
    public XmlRpcProviderInformation? Provider { get; set; }
    public Boolean OverridePostCount { get; set; }
    public Int32? FetchPostCount { get; set; }
    public String ExtraHeader { get; set; }
    public String ExtraFooter { get; set; }
    // editor
    [XmlArrayItem("CmdletObject")]
    public List<XmlPsCommand> Cmdlets { get; set; } = [];
    [XmlIgnore]
    public String ProjectPath { get; set; }


    public IReadOnlyList<IPsCommandInfo> GetCmdlets() {
        return Cmdlets;
    }
    public IXmlRpcProviderInformation? GetXmlRpcProviderInfo() {
        return Provider;
    }
}