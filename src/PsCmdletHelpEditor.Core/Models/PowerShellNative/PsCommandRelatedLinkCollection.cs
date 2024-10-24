using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

class PsCommandRelatedLinkCollection : IReadOnlyList<PsCommandRelatedLink> {
    readonly List<PsCommandRelatedLink> _list = [];

    /// <inheritdoc />
    public Int32 Count { get; set; }
    /// <inheritdoc />
    public PsCommandRelatedLink this[Int32 index] => _list[index];

    /// <inheritdoc />
    public IEnumerator<PsCommandRelatedLink> GetEnumerator() {
        return _list.GetEnumerator();
    }
    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public static PsCommandRelatedLinkCollection FromCommentBasedHelp(PSObject cbh) {
        var retValue = new PsCommandRelatedLinkCollection();
        var cbhRelatedLink = (PSObject)cbh.Members["relatedLinks"]?.Value;
        if (cbhRelatedLink is not null) {
            if (cbhRelatedLink.Members["navigationLink"].Value is PSObject singlePsObject) {
                retValue._list.Add(PsCommandRelatedLink.FromCommentBasedHelp(singlePsObject));
            } else {
                retValue._list.AddRange(((PSObject[])cbhRelatedLink.Members["navigationLink"].Value).Select(PsCommandRelatedLink.FromCommentBasedHelp));
            }
        }

        return retValue;
    }
}