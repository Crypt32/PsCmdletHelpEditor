using System;
using System.Collections.Generic;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

abstract class PsCommandParameterCollectionBase<T> : ReadOnlyCollectionBase<T> {
    static readonly List<String> _excludedParameters = [
        "Verbose",
        "Debug",
        "ErrorAction",
        "ErrorVariable",
        "OutVariable",
        "OutBuffer",
        "WarningVariable",
        "WarningAction",
        "PipelineVariable",
        "InformationAction",
        "InformationVariable"
    ];

    protected IReadOnlyCollection<String> ExcludedParameters => _excludedParameters;
}