using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PsCmdletHelpEditor.Core.Models.Xml;

[XmlInclude(typeof(XmlPsCommandParameterSet))]
[XmlInclude(typeof(XmlPsCommandParameterDescription))]
[XmlInclude(typeof(XmlPsCommandExample))]
[XmlInclude(typeof(XmlPsCommandRelatedLink))]
public class XmlPsCommand : IPsCommandInfo {
    public String Name { get; set; }
    [XmlAttribute("verb")]
    public String? Verb { get; set; }
    [XmlAttribute("noun")]
    public String? Noun { get; set; }
    public List<String> Syntax { get; set; } = [];
    [XmlElement("GeneralHelp")]
    public XmlPsCommandGeneralDescription GeneralHelp { get; set; }
    [XmlArrayItem("CommandParameterSetInfo2")]
    public List<XmlPsCommandParameterSet> ParamSets { get; set; } = [];
    [XmlArrayItem("ParameterDescription")]
    public List<XmlPsCommandParameterDescription> Parameters { get; set; } = [];
    [XmlArrayItem("Example")]
    public List<XmlPsCommandExample> Examples { get; set; } = [];
    [XmlArrayItem("RelatedLink")]
    public List<XmlPsCommandRelatedLink> RelatedLinks { get; set; } = [];
    [XmlElement("SupportInformation")]
    public XmlPsCommandSupportInfo? SupportInformation { get; set; }
    // advanced
    public String? ExtraHeader { get; set; }
    public String? ExtraFooter { get; set; }
    // rest
    public Boolean Publish { get; set; }
    public String? URL { get; set; }
    public String? ArticleIDString { get; set; }

    public IPsCommandGeneralDescription GetDescription() {
        return GeneralHelp;
    }
    public IReadOnlyList<String> GetSyntax() {
        return Syntax;
    }
    public IPsCommandSupportInfo? GetSupportInfo() {
        return SupportInformation;
    }
    public IReadOnlyList<IPsCommandParameterSetInfo> GetParameterSets() {
        return ParamSets;
    }
    public IReadOnlyList<IPsCommandParameterDescription> GetParameters() {
        return Parameters;
    }
    public IReadOnlyList<IPsCommandExample> GetExamples() {
        return Examples;
    }
    public IReadOnlyList<IPsCommandRelatedLink> GetRelatedLinks() {
        return RelatedLinks;
    }
}