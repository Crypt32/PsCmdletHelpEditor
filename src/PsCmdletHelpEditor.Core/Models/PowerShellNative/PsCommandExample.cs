using System;
using System.Linq;
using System.Management.Automation;

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
}