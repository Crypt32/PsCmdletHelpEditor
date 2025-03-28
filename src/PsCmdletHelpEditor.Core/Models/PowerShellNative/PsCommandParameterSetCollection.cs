﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

class PsCommandParameterSetCollection : PsCommandParameterCollectionBase<PsCommandParameterSet> {
    readonly Dictionary<String, CommandParameterInfo> _dictionary = new(StringComparer.OrdinalIgnoreCase);

    public CommandParameterInfo? GetParameterByName(String paramName) {
        _dictionary.TryGetValue(paramName, out CommandParameterInfo value);

        return value;
    }
    public IEnumerable<String> GetParamSetSyntax() {
        var retValue = new List<String>();
        var sb = new StringBuilder();
        foreach (PsCommandParameterSet paramSet in InternalList) {
            sb.Clear();
            foreach (String paramSetParameterName in paramSet.GetParameters().Where(_dictionary.ContainsKey)) {
                var paramInfo = _dictionary[paramSetParameterName];
                sb.Append(paramInfo.GetCommandSyntax());
            }
            retValue.Add(sb.ToString());
        }

        return retValue;
    }

    public void FromCmdlet(PSObject cmdlet) {
        InternalList.Clear();
        if (cmdlet.Members["ParameterSets"].Value != null) {
            var paramSets = (IEnumerable<CommandParameterSetInfo>)cmdlet.Members["ParameterSets"].Value;
            foreach (CommandParameterSetInfo paramSet in paramSets) {
                foreach (CommandParameterInfo paramInfo in paramSet.Parameters.Where(x => !ExcludedParameters.Contains(x.Name))) {
                    _dictionary[paramInfo.Name] = paramInfo;
                }
                InternalList.Add(PsCommandParameterSet.FromCmdlet(paramSet));
            }
        }
    }
}