using System.Linq;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

class PsCommandRelatedLinkCollection : ReadOnlyCollectionBase<PsCommandRelatedLink> {
    public void ImportCommentBasedHelp(PSObject cbh) {
        InternalList.Clear();
        var cbhRelatedLink = (PSObject)cbh.Members["relatedLinks"]?.Value;
        if (cbhRelatedLink is not null) {
            if (cbhRelatedLink.Members["navigationLink"].Value is PSObject singlePsObject) {
                InternalList.Add(PsCommandRelatedLink.FromCommentBasedHelp(singlePsObject));
            } else {
                InternalList.AddRange(((PSObject[])cbhRelatedLink.Members["navigationLink"].Value).Select(PsCommandRelatedLink.FromCommentBasedHelp));
            }
        }
    }
}