using System;
using System.Xml.Serialization;

namespace PsCmdletHelpEditor.Core.Models.Xml;

public class XmlPsCommandSupportInfo : IPsCommandSupportInfo{
    [XmlAttribute("ad")]
    public Boolean RequiresAD { get; set; }
    [XmlAttribute("rsat")]
    public Boolean RequiresRSAT { get; set; }
    [XmlAttribute("minPsVersion")]
    public Int32 PsVersionAsInt { get; set; }
    [XmlIgnore]
    public PsVersionSupport PsVersion => (PsVersionSupport)PsVersionAsInt;
    [XmlAttribute("winOsSupport")]
    public Int32 WinOsVersionAsInt { get; set; }
    [XmlIgnore]
    public WinOsVersionSupport WinOsVersion => (WinOsVersionSupport)WinOsVersionAsInt;
}