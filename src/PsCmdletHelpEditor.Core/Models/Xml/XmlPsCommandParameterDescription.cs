using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PsCmdletHelpEditor.Core.Models.Xml;

public class XmlPsCommandParameterDescription : IPsCommandParameterDescription {
    public String Name { get; set; } = null!;
    [XmlAttribute("type")]
    public String Type { get; set; } = null!;
    [XmlAttribute("varLen")]
    public Boolean AcceptsArray { get; set; }
    [XmlAttribute("required")]
    public Boolean Mandatory { get; set; }
    [XmlAttribute("dynamic")]
    public Boolean Dynamic { get; set; }
    [XmlAttribute("pipeRemaining")]
    public Boolean RemainingArgs { get; set; }
    [XmlAttribute("pipe")]
    public Boolean Pipeline { get; set; }
    [XmlAttribute("pipeProp")]
    public Boolean PipelinePropertyName { get; set; }
    [XmlAttribute("globbing")]
    public Boolean Globbing { get; set; }
    [XmlAttribute("isPos")]
    public Boolean Positional { get; set; }
    [XmlAttribute("pos")]
    public String? Position { get; set; }
    public String? Description { get; set; } = null!;
    public String? DefaultValue { get; set; }

    public List<String> Attributes { get; set; } = [];
    public List<String> Aliases { get; set; } = [];

    public IReadOnlyList<String> GetAttributes() {
        return Attributes;
    }
    public IReadOnlyList<String> GetAliases() {
        return Aliases;
    }
}