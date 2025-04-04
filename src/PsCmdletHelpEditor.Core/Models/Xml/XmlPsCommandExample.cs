﻿using System;

namespace PsCmdletHelpEditor.Core.Models.Xml;

public class XmlPsCommandExample : IPsCommandExample {
    public String Name { get; set; } = null!;
    public String? Cmd { get; set; } = null!;
    public String? Description { get; set; }
    public String? Output { get; set; }
}