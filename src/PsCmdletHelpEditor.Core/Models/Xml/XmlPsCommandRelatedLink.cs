using System;

namespace PsCmdletHelpEditor.Core.Models.Xml;

public class XmlPsCommandRelatedLink : IPsCommandRelatedLink {
    public String? LinkText { get; set; } = null!;
    public String? LinkUrl { get; set; }
}