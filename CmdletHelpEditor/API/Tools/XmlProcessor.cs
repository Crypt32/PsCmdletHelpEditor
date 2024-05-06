using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Abstractions;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.API.Templates;
using CodeKicker.BBCode;

namespace CmdletHelpEditor.API.Tools;
class XmlProcessor {
    readonly IProgressBar _progressBar;
    readonly BBCodeParser _bbRules;

    public XmlProcessor(IProgressBar progressBar) {
        _progressBar = progressBar;
        _bbRules = new HtmlProcessorV2().GetParser(ParserType.Clear);
    }

    // general
    static String generateParagraphs(String input, BBCodeParser bbRules) {
        if (String.IsNullOrEmpty(input)) {
            return "<maml:para />";
        }

        input = Regex.Replace(input, "(?<!\r)\n", "\r\n");
        String[] temp = input.Split(new[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (Int32 index = 0; index < temp.Length; index++) {
            temp[index] = bbRules.ToHtml(temp[index], true);
        }
        return "<maml:para>" + String.Join("</maml:para><maml:para>", temp) + "</maml:para>";
    }
    static String readParagraphs(XmlNodeList nodes) {
        if (nodes == null || nodes.Count == 0) { return String.Empty; }
        String retValue = nodes
            .Cast<XmlNode>()
            .Aggregate(String.Empty, (current, node) => current + Regex.Replace(node.InnerText, "(?<!\r)\n", "\r\n"));
        return retValue.TrimEnd();
    }
    // reader
    public static void ImportFromXml(String file, ModuleObject moduleObject) {
        if (String.IsNullOrEmpty(file)) { return; }
        var fileInfo = new FileInfo(file);
        String supDocPath = fileInfo.DirectoryName + "\\" + moduleObject.Name + ".supports.xml";
        fileInfo = new FileInfo(supDocPath);
        XmlNodeList nodes = null;
        var supDoc = new XmlDocument();
        try {
            if (fileInfo.Exists) {
                supDoc.Load(supDocPath);
                XmlNode selectSingleNode = supDoc.SelectSingleNode("SupportInformation");
                if (selectSingleNode == null) { throw new Exception(); }
                nodes = selectSingleNode.SelectNodes("Command");
            }
        } catch { }

        var doc = new XmlDocument();
        doc.Load(file);
        var ns = new XmlNamespaceManager(doc.NameTable);
        ns.AddNamespace("command", "http://schemas.microsoft.com/maml/dev/command/2004/10");
        ns.AddNamespace("maml", "http://schemas.microsoft.com/maml/2004/10");
        ns.AddNamespace("dev", "http://schemas.microsoft.com/maml/dev/2004/10");
        XmlNodeList commandNodes = doc.SelectNodes("//command:command", ns)
                                   ?? throw new XmlException("Error while reading XML.\nThe help file do not contains Command node.");
        var processed = new List<String>();
        foreach (XmlNode commandNode in commandNodes) {
            XmlNode nameNode = commandNode.SelectSingleNode("command:details/command:name", ns);
            if (nameNode == null) { continue; }
            Boolean isFound = false;
            foreach (CmdletObject cmdlet in moduleObject.Cmdlets) {
                if (String.Equals(nameNode.InnerText.Trim(), cmdlet.Name, StringComparison.CurrentCultureIgnoreCase) && !processed.Contains(cmdlet.Name.ToLower())) {
                    isFound = true;
                    processed.Add(cmdlet.Name);
                    readXml(commandNode, ns, cmdlet);
                    try {
                        if (fileInfo.Exists) {
                            readSupports(nodes, cmdlet);
                        }
                    } catch { }
                }
            }
            if (!isFound) {
                var currentCmdlet = new CmdletObject(nameNode.InnerText.Trim());
                readXml(commandNode, ns, currentCmdlet);
                moduleObject.Cmdlets.Add(currentCmdlet);
            }
        }
    }
    static void readXml(XmlNode commandNode, XmlNamespaceManager ns, CmdletObject currentCmdlet) {
        XmlNode tempNode = commandNode.SelectSingleNode("command:details/maml:description", ns);
        // Synopsis
        if (tempNode != null) {
            currentCmdlet.GeneralHelp.Synopsis = readParagraphs(tempNode.ChildNodes);
        }
        // Description
        tempNode = commandNode.SelectSingleNode("maml:description", ns);
        if (tempNode != null) {
            currentCmdlet.GeneralHelp.Description = readParagraphs(tempNode.ChildNodes);
        }
        if (currentCmdlet.GeneralHelp.Status != ItemStatus.Missing) {
            currentCmdlet.GeneralHelp.Status = String.IsNullOrEmpty(currentCmdlet.GeneralHelp.Synopsis) ||
                                               String.IsNullOrEmpty(currentCmdlet.GeneralHelp.Description)
                                                   ? ItemStatus.Incomplete
                                                   : ItemStatus.Valid;
        }
        // Notes
        tempNode = commandNode.SelectSingleNode("maml:alertSet/maml:alert", ns);
        if (tempNode != null) {
            currentCmdlet.GeneralHelp.Notes = readParagraphs(tempNode.ChildNodes);
        }
        // Input type
        XmlNodeList nodes = commandNode.SelectNodes("command:inputTypes/command:inputType", ns);
        readTypes(nodes, ns, currentCmdlet, false);
        // Output type
        nodes = commandNode.SelectNodes("command:returnValues/command:returnValue", ns);
        readTypes(nodes, ns, currentCmdlet, true);
        // Parameters
        nodes = commandNode.SelectNodes("command:parameters/command:parameter", ns);
        readParameters(nodes, ns, currentCmdlet);
        // Examples
        nodes = commandNode.SelectNodes("command:examples/command:example", ns);
        readExamples(nodes, ns, currentCmdlet);
        // Related links
        nodes = commandNode.SelectNodes("maml:relatedLinks/maml:navigationLink", ns);
        readLinks(nodes, ns, currentCmdlet);
    }
    static void readParameters(IEnumerable paramNodes, XmlNamespaceManager ns, CmdletObject currentCmdlet) {
        if (paramNodes == null) { return; }
        foreach (XmlNode paramNode in paramNodes) {
            var foundParam = new ParameterDescription();
            XmlNode tempNode = paramNode.SelectSingleNode("maml:name", ns);
            if (tempNode == null) { continue; }
            Boolean isFound = false;
            Int32 paramIndex = 0;
            for (Int32 index = 0; index < currentCmdlet.Parameters.Count; index++) {
                if (String.Equals(tempNode.InnerText.Trim(), currentCmdlet.Parameters[index].Name, StringComparison.CurrentCultureIgnoreCase)) {
                    paramIndex = index;
                    isFound = true;
                }
            }
            if (isFound) {
                // Parameter description
                tempNode = paramNode.SelectSingleNode("maml:description", ns);
                foundParam.Status = ItemStatus.Incomplete;
                if (tempNode != null) {
                    currentCmdlet.Parameters[paramIndex].Description = readParagraphs(tempNode.ChildNodes);
                    currentCmdlet.Parameters[paramIndex].Status = ItemStatus.Valid;
                }
                // Default value
                tempNode = paramNode.SelectSingleNode("dev:defaultValue", ns);
                if (tempNode != null) {
                    currentCmdlet.Parameters[paramIndex].DefaultValue = tempNode.InnerText.Trim();
                }
                // Globbing (aka wildcards)
                tempNode = paramNode.SelectSingleNode("@globbing", ns);
                currentCmdlet.Parameters[paramIndex].Globbing = tempNode != null && tempNode.Value.ToLower().Trim() == "true";
            } else {
                ParameterDescription newParam = new ParameterDescription {
                    Name = tempNode.InnerText.Trim()
                };
                tempNode = paramNode.SelectSingleNode("maml:description", ns);
                // Description
                if (tempNode != null) {
                    newParam.Description = readParagraphs(tempNode.ChildNodes);
                }
                // Default value
                tempNode = paramNode.SelectSingleNode("dev:defaultValue", ns);
                if (tempNode != null) {
                    newParam.DefaultValue = tempNode.InnerText.Trim();
                }
                // Globbing
                tempNode = paramNode.SelectSingleNode("@globbing", ns);
                if (tempNode != null) {
                    newParam.AcceptsArray = Convert.ToBoolean(tempNode.InnerText.Trim());
                }
                // Pipeline input
                tempNode = paramNode.SelectSingleNode("@pipelineInput", ns);
                if (tempNode != null) {
                    if (tempNode.InnerText.Trim().Contains("true")) { newParam.Pipeline = true; }
                    if (tempNode.InnerText.Trim().Contains("ByValue")) { newParam.Pipeline = true; }
                    if (tempNode.InnerText.Trim().Contains("ByPropertyName")) { newParam.PipelinePropertyName = true; }
                }
                // Parameter position
                tempNode = paramNode.SelectSingleNode("@position", ns);
                if (tempNode != null) {
                    newParam.Position = tempNode.Value.ToLower().Trim();
                    if (newParam.Position.ToLower() != "named") { newParam.Positional = true; }
                }
                // Mandatory
                tempNode = paramNode.SelectSingleNode("@required", ns);
                if (tempNode != null && tempNode.Value.ToLower().Trim() == "true") {
                    newParam.Mandatory = true;
                }
                // Parameter type
                tempNode = paramNode.SelectSingleNode("dev:type/maml:name", ns);
                if (tempNode != null) {
                    newParam.Type = tempNode.InnerText.Trim().ToLower();
                }
                currentCmdlet.Parameters.Add(newParam);
            }
        }
    }
    static void readTypes(IEnumerable typeNodes, XmlNamespaceManager ns, CmdletObject currentCmdlet, Boolean output) {
        if (typeNodes == null) { return; }
        var types = new List<String>();
        var url = new List<String>();
        var descriptions = new List<String>();
        foreach (XmlNode typeNode in typeNodes) {
            XmlNode tempNode = typeNode.SelectSingleNode("dev:type/maml:name", ns);
            if (tempNode == null) { continue; }
            types.Add(tempNode.InnerText.Trim());
            tempNode = typeNode.SelectSingleNode("dev:type/maml:uri", ns);
            if (tempNode != null) {
                url.Add(tempNode.InnerText.Trim());
            }
            String descriptionNodes = null;
            tempNode = typeNode.SelectSingleNode("dev:type/maml:description", ns);
            if (tempNode != null) {
                descriptionNodes += readParagraphs(tempNode.ChildNodes);
            }
            tempNode = typeNode.SelectSingleNode("maml:description", ns);
            if (tempNode != null) {
                descriptionNodes += readParagraphs(tempNode.ChildNodes);
            }
            descriptions.Add(descriptionNodes);
        }
        if (output) {
            currentCmdlet.GeneralHelp.ReturnType = String.Join(";", types);
            if (url.Count > 0) {
                currentCmdlet.GeneralHelp.ReturnUrl = String.Join(";", url);
            }
            if (descriptions.Count > 0) {
                currentCmdlet.GeneralHelp.ReturnTypeDescription = String.Join(";", descriptions);
            }
        } else {
            currentCmdlet.GeneralHelp.InputType = String.Join(";", types);
            if (url.Count > 0) {
                currentCmdlet.GeneralHelp.InputUrl = String.Join(";", url);
            }
            if (descriptions.Count > 0) {
                currentCmdlet.GeneralHelp.InputTypeDescription = String.Join(";", descriptions);
            }
        }
    }
    static void readExamples(IEnumerable exampleNodes, XmlNamespaceManager ns, CmdletObject currentCmdlet) {
        if (exampleNodes == null) { return; }
        foreach (XmlNode exampleNode in exampleNodes) {
            var example = new Example();
            // Example name
            XmlNode tempNode = exampleNode.SelectSingleNode("maml:title", ns);
            if (tempNode != null) {
                example.Name = tempNode.InnerText.Replace("-", String.Empty).Trim();
                if (String.IsNullOrEmpty(example.Name)) { example.Name = "Unknown"; }
            }
            // Example command
            tempNode = exampleNode.SelectSingleNode("dev:code", ns);
            if (tempNode != null) {
                example.Cmd = tempNode.InnerText.Trim();
            }
            // Example description
            tempNode = exampleNode.SelectSingleNode("dev:remarks", ns);
            if (tempNode != null) {
                Int32 NodeCount = 0;
                foreach (XmlNode DescriptionNode in tempNode) {
                    switch (NodeCount) {
                        case 0:
                            example.Description = readParagraphs(DescriptionNode.ChildNodes);
                            break;
                        case 3:
                            example.Output += DescriptionNode.InnerText.Trim();
                            break;
                    }
                    NodeCount++;
                }
            }
            // Example output
            tempNode = exampleNode.SelectSingleNode("command:commandLines", ns);
            if (tempNode != null) {
                example.Output += tempNode.InnerText.Trim();
            }
            currentCmdlet.Examples.Add(example);
        }
    }
    static void readLinks(IEnumerable linkNodes, XmlNamespaceManager ns, CmdletObject currentCmdlet) {
        if (linkNodes == null) { return; }
        foreach (XmlNode linkNode in linkNodes) {
            var link = new RelatedLink();
            // Link name
            XmlNode tempNode = linkNode.SelectSingleNode("maml:linkText", ns);
            if (tempNode != null) {
                link.LinkText = tempNode.InnerText.Trim();
                if (String.IsNullOrEmpty(link.LinkText)) { link.LinkText = "Unknown"; }
            }
            tempNode = linkNode.SelectSingleNode("maml:uri", ns);
            if (tempNode != null) {
                link.LinkUrl = tempNode.InnerText.Trim();
            }
            currentCmdlet.RelatedLinks.Add(link);
        }
    }
    static void readSupports(IEnumerable nodes, CmdletObject currentCmdlet) {
        foreach (XmlNode node in nodes) {
            if (node.Attributes == null || node.Attributes["name"].Value != currentCmdlet.Name) { continue; }
            XmlNodeList currentNode = node.SelectNodes("Info");
            if (currentNode == null) { continue; }
            foreach (XmlNode tempNode in currentNode) {
                if (tempNode.Attributes == null) { continue; }
                Boolean value = Convert.ToBoolean(tempNode.Attributes["value"].Value);
                switch (tempNode.Attributes["name"].Value.ToLower()) {
                    case "adrequired": currentCmdlet.SupportInformation.ADChecked = value; break;
                    case "rsatrequired": currentCmdlet.SupportInformation.RsatChecked = value; break;
                    case "ps2": currentCmdlet.SupportInformation.Ps2Checked = value; break;
                    case "ps3": currentCmdlet.SupportInformation.Ps3Checked = value; break;
                    case "ps4": currentCmdlet.SupportInformation.Ps4Checked = value; break;
                    case "winxp": currentCmdlet.SupportInformation.WinXpChecked = value; break;
                    case "winvista": currentCmdlet.SupportInformation.WinVistaChecked = value; break;
                    case "win7": currentCmdlet.SupportInformation.Win7Checked = value; break;
                    case "win8": currentCmdlet.SupportInformation.Win8Checked = value; break;
                    case "win81": currentCmdlet.SupportInformation.Win81Checked = value; break;
                    case "win10": currentCmdlet.SupportInformation.Win10Checked = value; break;
                    case "win11": currentCmdlet.SupportInformation.Win11Checked = value; break;
                    case "win2k3std": currentCmdlet.SupportInformation.Win2003StdChecked = value; break;
                    case "win2k3ee": currentCmdlet.SupportInformation.Win2003EEChecked = value; break;
                    case "win2k3dc": currentCmdlet.SupportInformation.Win2003DCChecked = value; break;
                    case "win2k8std": currentCmdlet.SupportInformation.Win2008StdChecked = value; break;
                    case "win2k8ee": currentCmdlet.SupportInformation.Win2008EEChecked = value; break;
                    case "win2k8dc": currentCmdlet.SupportInformation.Win2008DCChecked = value; break;
                    case "win2k8r2std": currentCmdlet.SupportInformation.Win2008R2StdChecked = value; break;
                    case "win2k8r2ee": currentCmdlet.SupportInformation.Win2008R2EEChecked = value; break;
                    case "win2k8r2dc": currentCmdlet.SupportInformation.Win2008R2DCChecked = value; break;
                    case "win2k12std": currentCmdlet.SupportInformation.Win2012StdChecked = value; break;
                    case "win2k12dc": currentCmdlet.SupportInformation.Win2012DCChecked = value; break;
                    case "win2k12r2std": currentCmdlet.SupportInformation.Win2012R2StdChecked = value; break;
                    case "win2k16std": currentCmdlet.SupportInformation.Win2016StdChecked = value; break;
                    case "win2k16dc": currentCmdlet.SupportInformation.Win2016DCChecked = value; break;
                    case "win2k19std": currentCmdlet.SupportInformation.Win2019StdChecked = value; break;
                    case "win2k19dc": currentCmdlet.SupportInformation.Win2019DCChecked = value; break;
                    case "win2k22std": currentCmdlet.SupportInformation.Win2022StdChecked = value; break;
                    case "win2k22dc": currentCmdlet.SupportInformation.Win2022DCChecked = value; break;
                }
            }
        }

    }

    // writer
    public static async Task<String> XmlGenerateHelp(IEnumerable<CmdletObject> cmdlets, IProgressBar pb, Boolean isOffline) {
        List<CmdletObject> cmdletsToProcess = isOffline
            ? new List<CmdletObject>(cmdlets)
            : new List<CmdletObject>(cmdlets.Where(x => x.GeneralHelp.Status != ItemStatus.Missing));
        if (cmdletsToProcess.Count == 0) { return String.Empty; }
        BBCodeParser bbRules = new HtmlProcessorV2().GetParser(ParserType.Clear);
        Double duration = 0;
        if (pb != null) {
            duration = 100.0 / cmdletsToProcess.Count;
        }
        var sb = new StringBuilder("<helpItems schema=\"maml\">");
        foreach (CmdletObject cmdlet in cmdletsToProcess) {
            await xmlGenerateBody(bbRules, sb, cmdlet);
            if (pb != null) {
                pb.Progress += duration;
            }
        }
        sb.Append("</helpItems>");
        return sb.ToString();
    }
    static Task xmlGenerateBody(BBCodeParser bbRules, StringBuilder SB, CmdletObject cmdlet) {
        return Task.Factory.StartNew(() => {
            SB.Append("<command:command xmlns:maml=\"http://schemas.microsoft.com/maml/2004/10\" xmlns:command=\"http://schemas.microsoft.com/maml/dev/command/2004/10\" xmlns:dev=\"http://schemas.microsoft.com/maml/dev/2004/10\" xmlns:MSHelp=\"http://msdn.microsoft.com/mshelp\">");
            SB.Append(xmlGenerateCmdletDetail(bbRules, cmdlet));
            SB.Append("<command:syntax>");
            // if current cmdlet hasn't parameters, then just write single syntaxItem
            if (cmdlet.Parameters.Count == 0) {
                SB.AppendFormat(XmlMamlTemplate.CommandSyntax,
                    SecurityElement.Escape(cmdlet.Name));
            } else {
                xmlGenerateParameterSyntax(bbRules, SB, cmdlet);
            }
            SB.Append("</command:syntax><command:parameters>");
            if (cmdlet.Parameters.Count > 0) {
                foreach (ParameterDescription item in cmdlet.Parameters.Where(item => item.Status != ItemStatus.Missing)) {
                    SB.Append(xmlGenerateParameter(bbRules, item));
                }
            }
            SB.Append("</command:parameters>");
            SB.Append(xmlGenerateInputTypes(bbRules, cmdlet));
            SB.Append(xmlGenerateReturnTypes(bbRules, cmdlet));
            SB.Append(XmlMamlTemplate.Error);
            SB.AppendFormat(XmlMamlTemplate.Note,
                generateParagraphs(cmdlet.GeneralHelp.Notes, bbRules));
            SB.Append("<command:examples>");
            foreach (IPsExample item in cmdlet.Examples) {
                SB.Append(xmlGenerateExamples(bbRules, item));
            }
            SB.Append("</command:examples><maml:relatedLinks>");
            foreach (IPsRelatedLink link in cmdlet.RelatedLinks) {
                SB.AppendFormat(XmlMamlTemplate.RelatedLink,
                    SecurityElement.Escape(link.LinkText),
                    SecurityElement.Escape(link.LinkUrl));
            }
            SB.Append("</maml:relatedLinks></command:command>");
        });
    }
    static String xmlGenerateCmdletDetail(BBCodeParser bbRules, CmdletObject cmdlet) {
        return String.Format(XmlMamlTemplate.Details,
            SecurityElement.Escape(cmdlet.Name),
            generateParagraphs(cmdlet.GeneralHelp.Synopsis, bbRules),
            SecurityElement.Escape(cmdlet.Verb),
            SecurityElement.Escape(cmdlet.Noun),
            generateParagraphs(cmdlet.GeneralHelp.Description, bbRules));
    }
    static void xmlGenerateParameterSyntax(BBCodeParser bbRules, StringBuilder SB, CmdletObject cmdlet) {
        String[] exclude = {
            "verbose","debug","erroraction","warningaction","errorvariable","warningvariable","outvariable","outbuffer","pipelinevariable"
        };
        foreach (CommandParameterSetInfo2 paramSet in cmdlet.ParamSets) {
            SB.Append("<command:syntaxItem>");
            SB.Append("<maml:name>" + SecurityElement.Escape(cmdlet.Name) + "</maml:name>");
            foreach (String paramSetParam in paramSet.Parameters) {
                if (exclude.Contains(paramSetParam.ToLower())) {
                    continue;
                }

                ParameterDescription param = null;
                String setParam = paramSetParam;
                foreach (ParameterDescription parameter in cmdlet.Parameters.Where(x => String.Equals(x.Name, setParam, StringComparison.CurrentCultureIgnoreCase))) {
                    param = parameter;
                }
                if (param == null) {
                    continue;
                }
                SB.Append("<command:parameter required=\"" + param.Mandatory.ToString().ToLower() + "\"");
                SB.Append(" variableLength=\"" + param.AcceptsArray.ToString().ToLower() + "\"");
                SB.Append(" globbing=\"" + param.Globbing.ToString().ToLower() + "\"");
                SB.Append(" pipelineInput=\"");
                if (param.Pipeline || param.PipelinePropertyName) {
                    SB.Append("true");
                    if (param.Pipeline && !param.PipelinePropertyName) {
                        SB.Append(" (ByValue)");
                    } else if (param.Pipeline && param.PipelinePropertyName) {
                        SB.Append(" (ByValue, ByPropertyName)");
                    } else {
                        SB.Append(" (ByPropertyName)");
                    }
                } else {
                    SB.Append("false");
                }
                SB.Append("\" position=\"" + param.Position + "\">");
                SB.Append("<maml:name>" + SecurityElement.Escape(param.Name) + "</maml:name>");
                SB.Append("<maml:description>");
                SB.Append(generateParagraphs(param.Description, bbRules));
                SB.Append("</maml:description>");
                SB.Append("<command:parameterValue ");
                String paramValueRequired = "true";
                if (param.Type.ToLower() == "boolean" || param.Type.ToLower() == "switchparameter") {
                    paramValueRequired = "false";
                }
                //Additional parameter Values
                SB.Append("required=\"" + paramValueRequired + "\"");
                SB.Append(" variableLength=\"" + param.AcceptsArray.ToString().ToLower() + "\">");
                SB.Append(SecurityElement.Escape(param.Type));
                SB.Append("</command:parameterValue>");
                SB.Append("</command:parameter>");
            }
            SB.Append("</command:syntaxItem>");
        }
    }
    static String xmlGenerateParameter(BBCodeParser bbRules, ParameterDescription param) {
        String paramValueRequired = "true";
        if (param.Type.ToLower() == "boolean" || param.Type.ToLower() == "switchparameter") {
            paramValueRequired = "false";
        }
        String paramValue = String.Format(XmlMamlTemplate.CommandParamValue,
            paramValueRequired,
            param.AcceptsArray.ToString().ToLower(),
            SecurityElement.Escape(param.Type));

        String pipelineInput = "false";
        if (param.Pipeline || param.PipelinePropertyName) {
            pipelineInput = "true";
            if (param.Pipeline && !param.PipelinePropertyName) {
                pipelineInput += " (ByValue)";
            } else if (param.Pipeline && param.PipelinePropertyName) {
                pipelineInput += " (ByValue, ByPropertyName)";
            } else {
                pipelineInput += " (ByPropertyName)";
            }
        }

        return String.Format(XmlMamlTemplate.CommandParam,
            param.Mandatory.ToString().ToLower(),
            param.AcceptsArray.ToString().ToLower(),
            param.Globbing.ToString().ToLower(),
            pipelineInput,
            param.Position,
            SecurityElement.Escape(param.Name),
            generateParagraphs(param.Description, bbRules),
            paramValue,
            SecurityElement.Escape(param.Type),
            SecurityElement.Escape(param.DefaultValue));
    }
    static String xmlGenerateInputTypes(BBCodeParser bbRules, CmdletObject cmdlet) {
        var inputTypes = new List<String>(cmdlet.GeneralHelp.InputType.Split(';'));
        var inputUrls = new List<String>(cmdlet.GeneralHelp.InputUrl.Split(';'));
        var inputDescription = new List<String>(cmdlet.GeneralHelp.InputTypeDescription.Split(';'));

        var SB = new StringBuilder("<command:inputTypes>");
        for (Int32 index = 0; index < inputTypes.Count; index++) {
            SB.AppendFormat(XmlMamlTemplate.InputType,
                bbRules.ToHtml(inputTypes[index], true),
                bbRules.ToHtml(inputUrls[index], true),
                generateParagraphs(inputDescription[index], bbRules));
        }
        SB.Append("	</command:inputTypes>");
        return SB.ToString();
    }
    static String xmlGenerateReturnTypes(BBCodeParser bbRules, CmdletObject cmdlet) {
        var returnTypes = new List<String>(cmdlet.GeneralHelp.ReturnType.Split(';'));
        var returnUrls = new List<String>(cmdlet.GeneralHelp.ReturnUrl.Split(';'));
        var returnDescription = new List<String>(cmdlet.GeneralHelp.ReturnTypeDescription.Split(';'));

        var SB = new StringBuilder("<command:returnValues>");
        for (Int32 index = 0; index < returnTypes.Count; index++) {
            SB.AppendFormat(XmlMamlTemplate.ReturnType,
                bbRules.ToHtml(returnTypes[index], true),
                bbRules.ToHtml(returnUrls[index], true),
                generateParagraphs(returnDescription[index], bbRules));
        }
        SB.Append("	</command:returnValues>");
        return SB.ToString();
    }
    static String xmlGenerateExamples(BBCodeParser bbRules, IPsExample example) {
        return String.Format(XmlMamlTemplate.Example,
            SecurityElement.Escape(example.Name),
            SecurityElement.Escape(example.Cmd),
            generateParagraphs(example.Description, bbRules),
            SecurityElement.Escape(example.Output));
    }
}