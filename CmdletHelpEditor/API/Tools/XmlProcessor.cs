using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using CmdletHelpEditor.API.Models;
using CodeKicker.BBCode;

namespace CmdletHelpEditor.API.Tools {
    static class XmlProcessor {
        static readonly String n = Environment.NewLine;
        // general
        static String generateParagraphs(String input, BBCodeParser bbRules, Int32 tabCount) {
            String tabs = new String('\t', tabCount);
            input = Regex.Replace(input, "(?<!\r)\n", "\r\n");
            if (String.IsNullOrEmpty(input)) { return tabs + "<maml:para />" + n; }
            String[] temp = input.Split(new[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (Int32 index = 0; index < temp.Length; index++) {
                temp[index] = bbRules.ToHtml(temp[index], true);
            }
            return tabs + "<maml:para>" + String.Join("</maml:para>" + n + tabs + "<maml:para>", temp) + "</maml:para>" + n;
        }
        static String readParagraphs(XmlNodeList nodes) {
            if (nodes == null || nodes.Count == 0) { return String.Empty; }
            String retValue = nodes
                .Cast<XmlNode>()
                .Aggregate(String.Empty, (current, node) => current + (Regex.Replace(node.InnerText, "(?<!\r)\n", "\r\n") + n + n));
            return retValue.TrimEnd();
        }
        // reader
        public static void ImportFromXml(String file, ModuleObject moduleObject) {
            if (String.IsNullOrEmpty(file)) { return; }
            FileInfo fileInfo = new FileInfo(file);
            String supDocPath = fileInfo.DirectoryName + "\\" + moduleObject.Name + ".supports.xml";
            fileInfo = new FileInfo(supDocPath);
            XmlNodeList nodes = null;
            XmlDocument supDoc = new XmlDocument();
            try {
                if (fileInfo.Exists) {
                    supDoc.Load(supDocPath);
                    XmlNode selectSingleNode = supDoc.SelectSingleNode("SupportInformation");
                    if (selectSingleNode == null) { throw new Exception(); }
                    nodes = selectSingleNode.SelectNodes("Command");    
                }
            } catch { }

            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("command", "http://schemas.microsoft.com/maml/dev/command/2004/10");
            ns.AddNamespace("maml", "http://schemas.microsoft.com/maml/2004/10");
            ns.AddNamespace("dev", "http://schemas.microsoft.com/maml/dev/2004/10");
            XmlNodeList commandNodes = doc.SelectNodes("//command:command", ns);
            if (commandNodes == null) {
                Utils.MsgBox("Error", "Error while reading XML.\nThe help file do not contains Command node.");
                return;
            }
            List<String> processed = new List<String>();
            foreach (XmlNode commandNode in commandNodes) {
                XmlNode nameNode = commandNode.SelectSingleNode("command:details/command:name", ns);
                if (nameNode == null) { continue; }
                Boolean isFound = false;
                foreach (CmdletObject cmdlet in moduleObject.Cmdlets) {
                    if (String.Equals(nameNode.InnerText.Trim(), cmdlet.Name, StringComparison.CurrentCultureIgnoreCase) && !processed.Contains(cmdlet.Name.ToLower())) {
                        isFound = true;
                        processed.Add(cmdlet.Name);
                        ReadXml(commandNode, ns, cmdlet);
                        try {
                            if (fileInfo.Exists) {
                                ReadSupports(nodes, cmdlet);
                            }
                        } catch { }
                    }
                }
                if (!isFound) {
                    CmdletObject currentCmdlet = new CmdletObject(nameNode.InnerText.Trim());
                    ReadXml(commandNode, ns, currentCmdlet);
                    moduleObject.Cmdlets.Add(currentCmdlet);
                }
            }
        }
        static void ReadXml(XmlNode commandNode, XmlNamespaceManager ns, CmdletObject currentCmdlet) {
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
            ReadTypes(nodes, ns, currentCmdlet, false);
            // Output type
            nodes = commandNode.SelectNodes("command:returnValues/command:returnValue", ns);
            ReadTypes(nodes, ns, currentCmdlet, true);
            // Parameters
            nodes = commandNode.SelectNodes("command:parameters/command:parameter", ns);
            ReadParameters(nodes, ns, currentCmdlet);
            // Examples
            nodes = commandNode.SelectNodes("command:examples/command:example", ns);
            ReadExamples(nodes, ns, currentCmdlet);
            // Related links
            nodes = commandNode.SelectNodes("maml:relatedLinks/maml:navigationLink", ns);
            ReadLinks(nodes, ns, currentCmdlet);
        }
        static void ReadParameters(IEnumerable paramNodes, XmlNamespaceManager ns, CmdletObject currentCmdlet) {
            if (paramNodes == null) { return; }
            foreach (XmlNode paramNode in paramNodes) {
                ParameterDescription foundParam = new ParameterDescription();
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
        static void ReadTypes(IEnumerable typeNodes, XmlNamespaceManager ns, CmdletObject currentCmdlet, Boolean output) {
            if (typeNodes == null) { return; }
            List<String> types = new List<String>();
            List<String> url = new List<String>();
            List<String> descriptions = new List<String>();
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
                    descriptionNodes += n + readParagraphs(tempNode.ChildNodes);
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
        static void ReadExamples(IEnumerable exampleNodes, XmlNamespaceManager ns, CmdletObject currentCmdlet) {
            if (exampleNodes == null) { return; }
            foreach (XmlNode exampleNode in exampleNodes) {
                Example example = new Example();
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
        static void ReadLinks(IEnumerable linkNodes, XmlNamespaceManager ns, CmdletObject currentCmdlet) {
            if (linkNodes == null) { return; }
            foreach (XmlNode linkNode in linkNodes) {
                RelatedLink link = new RelatedLink();
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
        static void ReadSupports(IEnumerable nodes, CmdletObject currentCmdlet) {
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
                        case "win2k12r2dc": currentCmdlet.SupportInformation.Win2012R2DCChecked = value; break;
                    }
                }
            }

        }

        // writer
        public static async Task XmlGenerateHelp(StringBuilder SB, IEnumerable<CmdletObject> cmdlets, ProgressBar pb, Boolean isOffline) {
            List<CmdletObject> cmdletsToProcess = isOffline
                ? new List<CmdletObject>(cmdlets)
                : new List<CmdletObject>(cmdlets.Where(x => x.GeneralHelp.Status != ItemStatus.Missing));
            if (cmdletsToProcess.Count == 0) { return; }
            BBCodeParser bbrules = HtmlProcessor.GetParser(ParserType.Clear);
            Double duration = 0;
            if (pb != null) {
                duration = 100.0 / cmdletsToProcess.Count;
            }
            SB.Append("<helpItems schema=\"maml\">" + n);
            foreach (CmdletObject cmdlet in cmdletsToProcess) {
                await XmlGenerateBody(bbrules, SB, cmdlet);
                if (pb != null) {
                    pb.Value += duration;
                }
            }
            SB.Append("</helpItems>");
        }
        static Task XmlGenerateBody(BBCodeParser bbRules, StringBuilder SB, CmdletObject cmdlet) {
            return Task.Factory.StartNew(() => {
                SB.Append("<command:command xmlns:maml=\"http://schemas.microsoft.com/maml/2004/10\" xmlns:command=\"http://schemas.microsoft.com/maml/dev/command/2004/10\" xmlns:dev=\"http://schemas.microsoft.com/maml/dev/2004/10\" xmlns:MSHelp=\"http://msdn.microsoft.com/mshelp\">" + n);
                XmlGenerateCmdletDetail(bbRules, SB, cmdlet);
                SB.Append("    <command:syntax>" + n);
                // if current cmdlet hasn't parameters, then just write single syntaxItem
                if (cmdlet.Parameters.Count == 0) {
                    SB.Append("        <command:syntaxItem>" + n);
                    SB.Append("            <maml:name>");
                    SB.Append(SecurityElement.Escape(cmdlet.Name));
                    SB.Append("</maml:name>" + n);
                    SB.Append("        </command:syntaxItem>" + n);
                } else {
                    XmlGenerateParameterSyntax(bbRules, SB, cmdlet);
                }
                SB.Append("    </command:syntax>" + n);
                SB.Append("    <command:parameters>");
                if (cmdlet.Parameters.Count > 0) {
                    SB.Append(n);
                    foreach (ParameterDescription item in cmdlet.Parameters.Where(item => item.Status != ItemStatus.Missing)) {
                        XmlGenerateParameter(bbRules, SB, item);
                    }
                }
                SB.Append("    </command:parameters>" + n);
                XmlGenerateInputTypes(bbRules, SB, cmdlet);
                XmlGenerateReturnTypes(bbRules, SB, cmdlet);
                XmlGenerateErrors(SB);
                XmlGenerateNotes(bbRules, SB, cmdlet);
                SB.Append("    <command:examples>" + n);
                if (cmdlet.Examples.Count > 0) {
                    foreach (Example item in cmdlet.Examples) {
                        XmlGenerateExamples(bbRules, SB, item);
                    }
                }
                SB.Append("    </command:examples>" + n);
                SB.Append("    <maml:relatedLinks>" + n);
                if (cmdlet.RelatedLinks.Count > 0) {
                    foreach (RelatedLink link in cmdlet.RelatedLinks) {
                        XmlGenerateLinks(SB, link);
                    }
                }
                SB.Append("    </maml:relatedLinks>" + n);
                SB.Append("</command:command>" + n);
            });
        }
        static void XmlGenerateCmdletDetail(BBCodeParser bbRules, StringBuilder SB, CmdletObject cmdlet) {
            SB.Append("<!--Generated by PS Cmdlet Help Editor-->" + n);
            SB.Append("    <command:details>" + n);
            SB.Append("        <command:name>");
            SB.Append(SecurityElement.Escape(cmdlet.Name));
            SB.Append("</command:name>" + n);
            // synopsis
            SB.Append("        <maml:description>" + n);
            SB.Append(generateParagraphs(cmdlet.GeneralHelp.Synopsis, bbRules, 3));
            SB.Append("        </maml:description>" + n);
            // TODO copyrights
            SB.Append("        <maml:copyright>" + n);
            SB.Append("            <maml:para />" + n);
            //SB.Append("        <!--Add copy right info here.-->" + n);
            SB.Append("        </maml:copyright>" + n);
            SB.Append("        <command:verb>");
            SB.Append(SecurityElement.Escape(cmdlet.Verb));
            SB.Append("</command:verb>" + n);
            SB.Append("        <command:noun>");
            SB.Append(SecurityElement.Escape(cmdlet.Noun));
            SB.Append("</command:noun>" + n);
            //dev version
            SB.Append("        <dev:version />" + n);
            //End </commnd:details>
            SB.Append("    </command:details>" + n);
            //Add Cmdlet detailed description
            SB.Append("    <maml:description>" + n);
            SB.Append(generateParagraphs(cmdlet.GeneralHelp.Description, bbRules, 2));
            SB.Append("    </maml:description>" + n);
        }
        static void XmlGenerateParameterSyntax(BBCodeParser bbRules, StringBuilder SB, CmdletObject cmdlet) {
            String[] exclude = {
                "verbose","debug","erroraction","warningaction","errorvariable","warningvariable","outvariable","outbuffer","pipelinevariable"
            };
            foreach (CommandParameterSetInfo2 paramSet in cmdlet.ParamSets) {
                SB.Append("        <command:syntaxItem>" + n);
                SB.Append("            <maml:name>" + SecurityElement.Escape(cmdlet.Name) + "</maml:name>" + n);
                foreach (String paramSetParam in paramSet.Parameters) {
                    if (exclude.Contains(paramSetParam.ToLower())) { continue; }
                    ParameterDescription param = null;
                    String setParam = paramSetParam;
                    foreach (ParameterDescription parameter in cmdlet.Parameters.Where(x => String.Equals(x.Name, setParam, StringComparison.CurrentCultureIgnoreCase))) {
                        param = parameter;
                    }
                    if (param == null) { continue; }
                    SB.Append("            <command:parameter required=\"" + param.Mandatory.ToString().ToLower() + "\"");
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
                    SB.Append("\" position=\"" + param.Position + "\">" + n);
                    SB.Append("                <maml:name>" + SecurityElement.Escape(param.Name) + "</maml:name>" + n);
                    SB.Append("                <maml:description>" + n);
                    SB.Append(generateParagraphs(param.Description, bbRules, 5));
                    SB.Append("                </maml:description>" + n);
                    SB.Append("                <command:parameterValue ");
                    String paramValueRequired = "true";
                    if (param.Type.ToLower() == "boolean" || param.Type.ToLower() == "switchparameter") {
                        paramValueRequired = "false";
                    }
                    //Additional parameter Values
                    SB.Append("required=\"" + paramValueRequired + "\"");
                    SB.Append(" variableLength=\"" + param.AcceptsArray.ToString().ToLower() + "\">");
                    SB.Append(SecurityElement.Escape(param.Type));
                    SB.Append("</command:parameterValue>" + n);
                    SB.Append("            </command:parameter>" + n);
                }
                SB.Append("        </command:syntaxItem>" + n);
            }
        }
        static void XmlGenerateParameter(BBCodeParser bbRules, StringBuilder SB, ParameterDescription param) {
            SB.Append("        <command:parameter required=\"" + param.Mandatory.ToString().ToLower() + "\"");
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
            SB.Append("\" position=\"" + param.Position + "\">" + n);
            SB.Append("            <maml:name>" + SecurityElement.Escape(param.Name) + "</maml:name>" + n);
            SB.Append("            <maml:description>" + n);
            SB.Append(generateParagraphs(param.Description, bbRules, 4));
            SB.Append("            </maml:description>" + n);
            SB.Append("            <command:parameterValue ");
            String paramValueRequired = "true";
            if (param.Type.ToLower() == "boolean" || param.Type.ToLower() == "switchparameter") {
                paramValueRequired = "false";
            }
            //Additional parameter Values
            SB.Append("required=\"" + paramValueRequired + "\"");
            SB.Append(" variableLength=\"" + param.AcceptsArray.ToString().ToLower() + "\">");
            SB.Append(SecurityElement.Escape(param.Type));
            SB.Append("</command:parameterValue>" + n);
            SB.Append("            <dev:type>" + n);
            SB.Append("                <maml:name>" + SecurityElement.Escape(param.Type) + "</maml:name>" + n);
            SB.Append("                <maml:uri/>" + n);
            SB.Append("            </dev:type>" + n);
            SB.Append("            <dev:defaultValue>" + SecurityElement.Escape(param.DefaultValue) + "</dev:defaultValue>" + n);
            SB.Append("        </command:parameter>" + n);
        }
        static void XmlGenerateInputTypes(BBCodeParser bbRules, StringBuilder SB, CmdletObject cmdlet) {
            List<String> inputTypes = new List<String>(cmdlet.GeneralHelp.InputType.Split(new[] { ';' }));
            List<String> inputUrls = new List<String>(cmdlet.GeneralHelp.InputUrl.Split(new[] { ';' }));
            List<String> inputDescription = new List<String>(cmdlet.GeneralHelp.InputTypeDescription.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            SB.Append("    <command:inputTypes>" + n);
            for (Int32 index = 0; index < inputTypes.Count; index++) {
                SB.Append("        <command:inputType>" + n);
                SB.Append("            <dev:type>" + n);
                SB.Append("                <maml:name>" + bbRules.ToHtml(inputTypes[index], true) + "</maml:name>" + n);
                try {
                    SB.Append("                <maml:uri>" + bbRules.ToHtml(inputUrls[index], true) + "</maml:uri>" + n);
                } catch {
                    SB.Append("                <maml:uri />" + n);
                }
                SB.Append("                <maml:description/>" + n);
                SB.Append("            </dev:type>" + n);
                SB.Append("            <maml:description>" + n);
                try {
                    SB.Append(generateParagraphs(inputDescription[index], bbRules, 4));
                } catch {
                    SB.Append("<maml:para />" + n);
                }
                SB.Append("            </maml:description>" + n);
                SB.Append("        </command:inputType>" + n);
            }
            SB.Append("    </command:inputTypes>" + n);
        }
        static void XmlGenerateReturnTypes(BBCodeParser bbRules, StringBuilder SB, CmdletObject cmdlet) {
            List<String> returnTypes = new List<String>(cmdlet.GeneralHelp.ReturnType.Split(new[] { ';' }));
            List<String> returnUrls = new List<String>(cmdlet.GeneralHelp.ReturnUrl.Split(new[] { ';' }));
            List<String> returnDescription = new List<String>(cmdlet.GeneralHelp.ReturnTypeDescription.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            SB.Append("    <command:returnValues>" + n);
            for (Int32 index = 0; index < returnTypes.Count; index++) {
                SB.Append("        <command:returnValue>" + n);
                SB.Append("            <dev:type>" + n);
                SB.Append("                <maml:name>" + bbRules.ToHtml(returnTypes[index], true) + "</maml:name>" + n);
                try {
                    SB.Append("                <maml:uri>" + bbRules.ToHtml(returnUrls[index], true) + "</maml:uri>" + n);
                } catch {
                    SB.Append("                <maml:uri />" + n);
                }
                SB.Append("                <maml:description/>" + n);
                SB.Append("            </dev:type>" + n);
                SB.Append("            <maml:description>" + n);
                try {
                    SB.Append(generateParagraphs(returnDescription[index], bbRules, 4));
                } catch {
                    SB.Append("<maml:para />" + n);
                }
                SB.Append("            </maml:description>" + n);
                SB.Append("        </command:returnValue>" + n);
            }
            SB.Append("    </command:returnValues>" + n);
        }
        static void XmlGenerateErrors(StringBuilder SB) {
            // TODO errors
            SB.Append("    <command:terminatingErrors></command:terminatingErrors>" + n);
            SB.Append("    <command:nonTerminatingErrors></command:nonTerminatingErrors>" + n);
        }
        static void XmlGenerateNotes(BBCodeParser bbRules, StringBuilder SB, CmdletObject cmdlet) {
            SB.Append("    <maml:alertSet>" + n);
            SB.Append("        <maml:title></maml:title>" + n);
            SB.Append("        <maml:alert>" + n);
            SB.Append(generateParagraphs(cmdlet.GeneralHelp.Notes, bbRules, 3));
            SB.Append("        </maml:alert>" + n);
            SB.Append("    </maml:alertSet>" + n);
        }
        static void XmlGenerateExamples(BBCodeParser bbRules, StringBuilder SB, Example example) {
            SB.Append("        <command:example>" + n);
            SB.Append("            <maml:title>--------------------------  " + SecurityElement.Escape(example.Name) + "  --------------------------</maml:title>" + n);
            SB.Append("            <maml:introduction>" + n);
            SB.Append("                <maml:paragraph>PS C:\\&gt;</maml:paragraph>" + n);
            SB.Append("            </maml:introduction>" + n);
            SB.Append("            <dev:code>");
            SB.Append(SecurityElement.Escape(example.Cmd) + "</dev:code>" + n);
            SB.Append("            <dev:remarks>" + n);
            SB.Append(generateParagraphs(example.Description, bbRules, 4));
            SB.Append("                <maml:para />" + n);
            SB.Append("                <maml:para />" + n);
            SB.Append("                <maml:para>" + SecurityElement.Escape(example.Output) + "</maml:para>" + n);
            SB.Append("            </dev:remarks>" + n);
            SB.Append("            <command:commandLines>" + n);
            SB.Append("                <command:commandLine>" + n);
            SB.Append("                    <command:commandText>" + n);
            SB.Append("                        <maml:para />" + n);
            SB.Append("                    </command:commandText>" + n);
            SB.Append("                </command:commandLine>" + n);
            SB.Append("            </command:commandLines>" + n);
            SB.Append("        </command:example>" + n);
        }
        static void XmlGenerateLinks(StringBuilder SB, RelatedLink link) {
            SB.Append("        <maml:navigationLink>" + n);
            SB.Append("            <maml:linkText>" + SecurityElement.Escape(link.LinkText) + "</maml:linkText>" + n);
            SB.Append("            <maml:uri>" + SecurityElement.Escape(link.LinkUrl) + "</maml:uri>" + n);
            SB.Append("        </maml:navigationLink>" + n);
        }
    }
}
