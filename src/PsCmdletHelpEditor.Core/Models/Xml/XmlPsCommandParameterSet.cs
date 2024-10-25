using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PsCmdletHelpEditor.Core.Models.Xml;

public class XmlPsCommandParameterSet : IPsCommandParameterSetInfo {
    [XmlAttribute]
    public String Name { get; set; }
    [XmlAttribute("Params")]
    public List<String> Parameters { get; set; }

    public IReadOnlyList<String> GetParameters() {
        return Parameters;
    }
}