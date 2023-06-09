using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CmdletHelpEditor.API.Abstractions;

namespace CmdletHelpEditor.API.Models;

public class CommandParameterSetInfo2 : IParameterSetInfo {
    public CommandParameterSetInfo2() {
        Parameters = new List<String>();
    }
    [XmlAttribute]
    public String Name { get; set; }
    [XmlAttribute("Params")]
    public List<String> Parameters { get; set; }
}
