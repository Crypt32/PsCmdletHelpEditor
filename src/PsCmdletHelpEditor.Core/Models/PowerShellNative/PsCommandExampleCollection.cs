using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

class PsCommandExampleCollection : IReadOnlyList<PsCommandExample> {
    readonly List<PsCommandExample> _list = [];

    /// <inheritdoc />
    public Int32 Count { get; set; }
    /// <inheritdoc />
    public PsCommandExample this[Int32 index] => _list[index];

    /// <inheritdoc />
    public IEnumerator<PsCommandExample> GetEnumerator() {
        return _list.GetEnumerator();
    }
    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public static PsCommandExampleCollection FromCommentBasedHelp(PSObject cbh) {
        var retValue = new PsCommandExampleCollection();
        var cbhExample = (PSObject)cbh.Members["examples"]?.Value;
        if (cbhExample is not null) {
            if (cbhExample.Members["example"].Value is PSObject singlePsObject) {
                retValue._list.Add(PsCommandExample.FromCommentBasedHelp(singlePsObject));
            } else {
                retValue._list.AddRange(((PSObject[])cbhExample.Members["navigationLink"].Value).Select(PsCommandExample.FromCommentBasedHelp));
            }
        }

        return retValue;
    }
}