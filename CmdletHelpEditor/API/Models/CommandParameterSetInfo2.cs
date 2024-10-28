﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Models.Xml;

namespace CmdletHelpEditor.API.Models;

public class CommandParameterSetInfo2 {
    [XmlAttribute]
    public String Name { get; set; }
    [XmlAttribute("Params")]
    public List<String> Parameters { get; set; } = [];

    public XmlPsCommandParameterSet ToXml() {
        return new XmlPsCommandParameterSet {
            Name = Name,
            Parameters = Parameters
        };
    }

    public static CommandParameterSetInfo2 FromCommandInfo(IPsCommandParameterSetInfo paramSet) {
        var retValue = new CommandParameterSetInfo2 {
            Name = paramSet.Name
        };
        retValue.Parameters.AddRange(paramSet.GetParameters());

        return retValue;
    }
}
