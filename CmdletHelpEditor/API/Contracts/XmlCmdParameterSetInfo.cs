using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CmdletHelpEditor.API.Abstractions;

namespace CmdletHelpEditor.API.Contracts;

public class XmlCmdParameterSetInfo: IParameterSetInfo {
    [XmlAttribute]
    public String Name { get; set; }
    [XmlAttribute("Params")]
    public List<String> Parameters { get; set; } = new();
}