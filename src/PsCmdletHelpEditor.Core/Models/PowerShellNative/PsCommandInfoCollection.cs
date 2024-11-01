using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using PsCmdletHelpEditor.Core.Models.Xml;
using PsCmdletHelpEditor.Core.Services.MAML;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;
class PsCommandInfoCollection : ReadOnlyCollectionBase<PsCommandInfo> {
    public void ImportFromCmdletList(IEnumerable<PSObject> commands, Boolean includeCommentBasedHelp, String? mamlHelpPath) {
        var psCommandsNames = new HashSet<String>(StringComparer.OrdinalIgnoreCase);
        foreach (PSObject commandObject in commands) {
            var command = PsCommandInfo.FromCommandInfo(commandObject, includeCommentBasedHelp);
            psCommandsNames.Add(command.Name);
            InternalList.Add(command);
        }

        if (!includeCommentBasedHelp && mamlHelpPath is not null) {
            var mamlService = new PsMamlService();
            IDictionary<String, MamlXmlNode> mamlCommands = mamlService.GetMamlHelpFromFile(mamlHelpPath);
            // import MAML help only for existing commands
            foreach (PsCommandInfo command in InternalList) {
                if (mamlCommands.TryGetValue(command.Name, out MamlXmlNode commandNode)) {
                    command.ImportMamlHelp(commandNode);
                }
            }
            // MAML may contain commands that no longer exist in module. Add them as well and flag.
            foreach (String mamlOnlyCommandName in mamlCommands.Keys.Except(psCommandsNames)) {
                PsCommandInfo command = PsCommandInfo.FromMamlHelp(mamlOnlyCommandName, mamlCommands[mamlOnlyCommandName]);
                // flag as orphaned
                InternalList.Add(command);
            }
        }
    }
}
