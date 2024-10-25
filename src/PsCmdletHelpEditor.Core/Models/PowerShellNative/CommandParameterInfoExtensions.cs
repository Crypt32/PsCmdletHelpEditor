using System;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;
static class CommandParameterInfoExtensions {
    /// <summary>
    /// Generates PowerShell parameter syntax string.
    /// </summary>
    /// <param name="param">PowerShell parameter to generate syntax string for.</param>
    /// <returns>Parameter syntax string.</returns>
    public static String GetCommandSyntax(this CommandParameterInfo param) {
        Boolean named = param.Position < 0;
        // fetch param type
        String paramType = param.ParameterType.Name;
        // fetch ValidateSet attribute
        String validateSet = String.Empty;
        foreach (Attribute attribute in param.Attributes) {
            Boolean found = false;
            validateSet = String.Empty;
            switch (attribute.TypeId.ToString()) {
                case "System.Management.Automation.ValidateSetAttribute":
                    validateSet += " {";
                    validateSet += String.Join(" | ", ((ValidateSetAttribute)attribute).ValidValues);
                    validateSet += "} ";
                    found = true;
                    break;
            }
            if (found) { break; }
        }
        if (param.IsMandatory && named) {
            return $" -{param.Name} <{paramType}>{validateSet}";
        }
        if (param.IsMandatory) {
            return $" [-{param.Name}] <{paramType}>{validateSet}";
        }
        if (!named) {
            return $" [[-{param.Name}] <{paramType}>{validateSet}]";
        }
        if (!String.IsNullOrEmpty(paramType) && paramType != "SwitchParameter") {
            return $" [-{param.Name} <{paramType}>{validateSet}]";
        }

        return $" [-{param.Name}{validateSet}]";
    }
}
