using System.Linq;
using System.Management.Automation;
using PsCmdletHelpEditor.Core.Models.Xml;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

class PsCommandRelatedLinkCollection : ReadOnlyCollectionBase<PsCommandRelatedLink> {
    public void ImportCommentBasedHelp(PSObject cbh) {
        InternalList.Clear();
        var cbhRelatedLink = (PSObject)cbh.Members["relatedLinks"]?.Value;
        if (cbhRelatedLink is not null) {
            if (cbhRelatedLink.Members["navigationLink"].Value is PSObject singlePsObject) {
                InternalList.Add(PsCommandRelatedLink.FromCommentBasedHelp(singlePsObject));
            } else {
                InternalList.AddRange(((PSObject[])cbhRelatedLink.Members["navigationLink"].Value)
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