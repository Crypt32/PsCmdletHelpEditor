using System;

namespace CmdletHelpEditor.API.Contracts;

public class XmlCmdGeneralDescription {
    public String Synopsis { get; set; }
    public String Description { get; set; }
    public String Notes { get; set; }
    public String InputType { get; set; }
    public String InputUrl { get; set; }
    public String InputTypeDescription { get; set; }
    public String ReturnType { get; set; }
    public String ReturnUrl { get; set; }
    public String ReturnTypeDescription { get; set; }
}