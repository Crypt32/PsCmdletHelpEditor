﻿using System.Linq;
using System.Management.Automation;
using PsCmdletHelpEditor.Core.Models.Xml;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

class PsCommandRelatedLinkCollection : ReadOnlyCollectionBase<PsCommandRelatedLink> {
    public void ImportCommentBasedHelp(PSObject cbh) {
        InternalList.Clear();
        var cbhRelatedLinks = (PSObject)cbh.Members["relatedLinks"]?.Value;
        if (cbhRelatedLinks?.Members["navigationLink"] != null) {
            if (cbhRelatedLinks.Members["navigationLink"]?.Value is PSObject singlePsObject) {
                InternalList.Add(PsCommandRelatedLink.FromCommentBasedHelp(singlePsObject));
            } else {
                InternalList.AddRange(((PSObject[])cbhRelatedLinks.Members["navigationLink"].Value)
                    .Select(PsCommandRelatedLink.FromCommentBasedHelp));
            }
        }
    }
    public void ImportMamlHelp(MamlXmlNode commandNode) {
        InternalList.Clear();
        MamlXmlNodeList? nodes = commandNode.SelectNodes("maml:relatedLinks/maml:navigationLink");
        if (nodes == null) {
            return;
        }
        foreach (MamlXmlNode node in nodes) {
            var link = PsCommandRelatedLink.FromMamlHelp(node);
            if (link is not null) {
                InternalList.Add(link);
            }
        }
    }
}