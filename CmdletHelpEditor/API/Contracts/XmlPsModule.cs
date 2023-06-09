using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Xml.Serialization;

namespace CmdletHelpEditor.API.Contracts;

[XmlRoot("ModuleObject")]
[XmlInclude(typeof(XmlPsCmdlet))]
public class XmlPsModule {
    [XmlAttribute("fVersion")]
    public Double FormatVersion { get; set; }
    public String Name { get; set; }
    [XmlAttribute("type")]
    public ModuleType ModuleType { get; set; }
    public String Version { get; set; }
    public String Description { get; set; }
    [XmlAttribute("mclass")]
    public String ModuleClass { get; set; }
    [XmlAttribute("useSupports")]
    public Boolean UseSupports { get; set; }
    public Boolean HasManifest { get; set; }
    public XmlMwProviderInfo Provider { get; set; } = new();
    public Boolean OverridePostCount { get; set; }
    public Int32? FetchPostCount { get; set; }
    public String ExtraHeader { get; set; }
    public String ExtraFooter { get; set; }
    [XmlArrayItem("CmdletObject")]
    public List<XmlPsCmdlet> Cmdlets { get; set; } = new();
}