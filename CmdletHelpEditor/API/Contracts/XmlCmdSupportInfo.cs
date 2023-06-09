using System;
using System.Xml.Serialization;

namespace CmdletHelpEditor.API.Contracts;

public class XmlCmdSupportInfo {
    [XmlAttribute("ad")]
    public Boolean RequiresAD { get; set; }
    [XmlAttribute("rsat")]
    public Boolean RequiresRSAT { get; set; }
    [XmlAttribute("minPsVersion")]
    public Int32 PsVersionAsInt { get; set; }
    [XmlAttribute("winOsSupport")]
    public Int32 WinOsVersionAsInt { get; set; }
}