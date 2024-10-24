using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

class PsCommandParameterSetCollection : IReadOnlyList<PsCommandParameterSet> {
    readonly List<PsCommandParameterSet> _list = [];

    /// <inheritdoc />
    public Int32 Count { get; set; }
    /// <inheritdoc />
    public PsCommandParameterSet this[Int32 index] => _list[index];

    /// <inheritdoc />
    public IEnumerator<PsCommandParameterSet> GetEnumerator() {
        return _list.GetEnumerator();
    }
    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public void FromCmdlet(PSObject cmdlet) {
        _list.Clear();
        if (cmdlet.Members["ParameterSets"].Value != null) {
            var paramSets = (IEnumerable<CommandParameterSetInfo>)cmdlet.Members["ParameterSets"].Value;
            foreach (CommandParameterSetInfo paramSet in paramSets) {
                _list.Add(PsCommandParameterSet.FromCmdlet(paramSet));
            }
        }
    }
}