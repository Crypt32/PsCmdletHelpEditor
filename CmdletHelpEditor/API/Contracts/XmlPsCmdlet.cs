using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CmdletHelpEditor.API.Contracts;

[XmlInclude(typeof(XmlCmdParameterDescription))]
[XmlInclude(typeof(XmlCmdExample))]
[XmlInclude(typeof(XmlCmdRelatedLink))]
[XmlInclude(typeof(XmlCmdParameterSetInfo))]
public class XmlPsCmdlet {
    public String Name { get; set; }
    [XmlAttribute("verb")]
    public String Verb { get; set; }
    [XmlAttribute("noun")]
    public String Noun { get; set; }
    public List<String> Syntax { get; set; } = new();
    [XmlElement("GeneralHelp")]
    public XmlCmdGeneralDescription GeneralHelp { get; set; } = new();
    [XmlArrayItem("CommandParameterSetInfo2")]
    public List<XmlCmdParameterSetInfo> ParamSets { get; set; } = new();
    [XmlArrayItem("ParameterDescription")]
    public List<XmlCmdParameterDescription> Parameters { get; set; } = new();
    [XmlArrayItem("Example")]
    public List<XmlCmdExample> Examples { get; set; } = new();
    [XmlArrayItem("RelatedLink")]
    public List<XmlCmdRelatedLink> RelatedLinks { get; set; } = new();
    [XmlElement("SupportInformation")]
    public XmlCmdSupportInfo SupportInformation { get; set; } = new();
    // advanced
    public String ExtraHeader { get; set; }
    public String ExtraFooter { get; set; }
    // rest
    public Boolean Publish { get; set; }
    public String URL { get; set; }
    public String ArticleIDString { get; set; }
}