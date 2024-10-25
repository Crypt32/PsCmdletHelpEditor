using System.Linq;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

class PsCommandExampleCollection : ReadOnlyCollectionBase<PsCommandExample> {
    public void ImportCommentBasedHelp(PSObject cbh) {
        InternalList.Clear();
        var cbhExample = (PSObject)cbh.Members["examples"]?.Value;
        if (cbhExample is not null) {
            if (cbhExample.Members["example"].Value is PSObject singlePsObject) {
                InternalList.Add(PsCommandExample.FromCommentBasedHelp(singlePsObject));
            } else {
                InternalList.AddRange(((PSObject[])cbhExample.Members["example"].Value).Select(PsCommandExample.FromCommentBasedHelp));
            }
        }
    }
}