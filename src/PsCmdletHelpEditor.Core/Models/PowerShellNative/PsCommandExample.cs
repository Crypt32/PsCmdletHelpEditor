using System;
using System.Linq;
using System.Management.Automation;
using PsCmdletHelpEditor.Core.Models.Xml;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

class PsCommandExample : IPsCommandExample {
    public String Name { get; set; } = null!;
    public String? Cmd { get; set; }
    public String? Description { get; set; }
    public String? Output { get; set; }

    public static PsCommandExample FromCommentBasedHelp(PSObject cbh) {
        // replace dashes, as some examples are formatted as '------- Example X ---------'.
        String title = ((String)((PSObject)cbh.Members["title"].Value).BaseObject).Replace("-", String.Empty).Trim();
        String code = (String)((PSObject)cbh.Members["code"].Value).BaseObject;
        String description = ((PSObject[])cbh.Members["remarks"].Value)
            .Aggregate(String.Empty, (current, paragraph) => current + paragraph.Members["Text"].Value + Environment.NewLine);

        return new PsCommandExample {
            Name = title,
            Cmd = code,
            Description = description
        };
    }

    public static PsCommandExample? FromMamlHelp(MamlXmlNode node) {
        String? cmd = null;
        String? description = null;
        String? output = null;
        MamlXmlNode? tempNode = node.SelectSingleNode("maml:title");
        if (tempNode is null) {
            return null;
        }

        String name = tempNode.InnerText.Replace("-", String.Empty);
        if (String.IsNullOrEmpty(name)) {
            name = "Unknown";
        }
        // Example command
        tempNode = node.SelectSingleNode("dev:code");
        if (tempNode != null) {
            cmd = tempNode.InnerText;
        }
        // Example description
        tempNode = node.SelectSingleNode("dev:remarks");
        if (tempNode?.ChildNodes.Count > 0) {
            Int32 NodeCount = 0;
            foreach (MamlXmlNode descriptionNode in tempNode.ChildNodes) {
                switch (NodeCount) {
                    case 0:
                        description = descriptionNode.ChildNodes.ReadMamlParagraphs();
                        break;
                    case 3:
                        output += descriptionNode.InnerText;
                        break;
                }
                NodeCount++;
            }
        }
        // Example output
        tempNode = node.SelectSingleNode("command:commandLines");
        if (tempNode != null) {
            output += tempNode.InnerText;
        }

        return new PsCommandExample {
            Name = name.Trim(),
            Cmd = cmd,
            Description = description,
            Output = output
        };
    }
}