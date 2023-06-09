using System;
using CmdletHelpEditor.API.Abstractions;

namespace CmdletHelpEditor.API.Contracts;

public class XmlCmdExample : IPsExample {
    public String Name { get; set; }
    public String Cmd { get; set; }
    public String Description { get; set; }
    public String Output { get; set; }
}