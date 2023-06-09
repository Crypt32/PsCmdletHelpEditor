using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CmdletHelpEditor.API.Contracts;

public class XmlCmdParameterDescription {
    public String Name { get; set; }
    [XmlAttribute("type")]
    public String Type { get; set; }
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
    [XmlAttribute("isPos")]
    public Boolean Positional { get; set; }
    [XmlAttribute("pos")]
    public String Position { get; set; }
    public List<String> Attributes { get; set; } = new();
    public List<String> Aliases { get; set; } = new();
    public String Description { get; set; }
    public String DefaultValue { get; set; }
    [XmlAttribute("globbing")]
    public Boolean Globbing { get; set; }
}