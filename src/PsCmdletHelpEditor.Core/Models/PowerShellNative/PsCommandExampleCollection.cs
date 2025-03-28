using System.Linq;
using System.Management.Automation;
using PsCmdletHelpEditor.Core.Models.Xml;

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

    public void ImportMamlHelp(MamlXmlNode commandNode) {
        InternalList.Clear();
        MamlXmlNodeList? nodes = commandNode.SelectNodes("command:examples/command:example");
        if (nodes == null) {
            return;
        }
        foreach (MamlXmlNode node in nodes) {
            PsCommandExample? example = PsCommandExample.FromMamlHelp(node);
            if (example is not null) {
                InternalList.Add(example);
            }
        }
    }
}