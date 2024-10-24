using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

class PsCommandParameterSet : IPsCommandParameterSetInfo {
    readonly List<String> _parameters = [];

    PsCommandParameterSet() { }

    public String Name { get; private set; } = null!;

    public IReadOnlyList<String> GetParameters() {
        return _parameters;
    }

    public static PsCommandParameterSet FromCmdlet(CommandParameterSetInfo paramSet) {
        var retValue = new PsCommandParameterSet {
            Name = paramSet.Name
        };
        foreach (CommandParameterInfo param in paramSet.Parameters) {
            retValue._parameters.Add(param.Name);
        }

        return retValue;
    }
}