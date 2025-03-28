using System;
using System.Management.Automation;
using PsCmdletHelpEditor.Core.Models.Xml;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

class PsCommandRelatedLink : IPsCommandRelatedLink {
    PsCommandRelatedLink(String linkText, String? linkUrl) {
        LinkText = linkText;
        LinkUrl = linkUrl;
    }

    public String LinkText { get; }
    public String? LinkUrl { get; }

    public static PsCommandRelatedLink FromCommentBasedHelp(PSObject cbh) {
        String linkText = (String)((PSObject)cbh.Members["linkText"]?.Value)?.BaseObject ?? "Unknown";
        String? linkUrl = (String)((PSObject)cbh.Members["uri"]?.Value)?.BaseObject;

        return new PsCommandRelatedLink(linkText, linkUrl);
    }
    public static PsCommandRelatedLink? FromMamlHelp(MamlXmlNode node) {
        String? linkText;
        String? linkUrl = null;
        MamlXmlNode? tempNode = node.SelectSingleNode("maml:linkText");
        if (tempNode != null) {
            linkText = tempNode.InnerText;
            if (String.IsNullOrEmpty(linkText)) {
                linkText = "Unknown";
            }
        } else {
            return null;
        }
        tempNode = node.SelectSingleNode("maml:uri");
        if (tempNode != null) {
            linkUrl = tempNode.InnerText;
        }

        return new PsCommandRelatedLink(linkText, linkUrl);
    }
}