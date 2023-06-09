using System;
using CmdletHelpEditor.API.Abstractions;

namespace CmdletHelpEditor.API.Contracts;

public class XmlCmdRelatedLink : IPsRelatedLink {
    public String LinkText { get; set; }
    public String LinkUrl { get; set; }
}