using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

class PsCommandParameterCollection : PsCommandParameterBase, IReadOnlyList<PsCommandParameter> {
    readonly List<PsCommandParameter> _list = [];

    /// <inheritdoc />
    public Int32 Count { get; set; }
    /// <inheritdoc />
    public PsCommandParameter this[Int32 index] => _list[index];

    /// <inheritdoc />
    public IEnumerator<PsCommandParameter> GetEnumerator() {
        return _list.GetEnumerator();
    }
    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public void FromCmdlet(PSObject cmdlet) {
        if (cmdlet.Members["ParameterSets"].Value != null) {
            var paramSets = new List<CommandParameterSetInfo>((IEnumerable<CommandParameterSetInfo>)cmdlet.Members["ParameterSets"].Value);
            foreach (CommandParameterSetInfo paramSet in paramSets) {
                if (paramSet.Parameters.Count == 0) { return; }

                foreach (CommandParameterInfo param in paramSet.Parameters.Where(param => !ExcludedParameters.Contains(param.Name, StringComparer.OrdinalIgnoreCase))) {
                    var psParam = PsCommandParameter.FromCmdlet(param);
                    if (!_list.Contains(psParam)) {
                        _list.Add(psParam);
                    }
                }
            }
        }
    }
}