using System;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

class PsCommandRelatedLink : IPsCommandRelatedLink {
    PsCommandRelatedLink() { }

    public String? LinkText { get; private set; }
    public String? LinkUrl { get; private set; }

    public static PsCommandRelatedLink FromCommentBasedHelp(PSObject cbh) {
        String linkText = (String)((PSObject)cbh.Members["linkText"].Value).BaseObject;
        String uri = (String)((PSObject)cbh.Members["uri"].Value).BaseObject;

        return new PsCommandRelatedLink {
            LinkText = linkText,
            LinkUrl = uri
        };
    }
}