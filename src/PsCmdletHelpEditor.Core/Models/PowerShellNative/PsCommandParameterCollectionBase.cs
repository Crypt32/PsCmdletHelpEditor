using System;
using System.Collections.Generic;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

abstract class PsCommandParameterCollectionBase<T> : ReadOnlyCollectionBase<T> {
    protected static List<String> ExcludedParameters = [
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
}