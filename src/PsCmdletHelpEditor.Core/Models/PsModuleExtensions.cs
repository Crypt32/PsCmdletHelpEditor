using System;
using System.Collections.Generic;

namespace PsCmdletHelpEditor.Core.Models;
static class PsModuleExtensions {
    const String INVOCATION_STRING_TEMPLATE = """
                                              $Error.Clear()
                                              {0} -Name "{2}"{3} -ErrorAction Stop
                                              if ((Get-Module -Name {1}) -eq $null) {{
                                                  throw New-Object Exception $Error[0].Exception.Message
                                              }}
                                              Get-Command -Module {1} -CommandType {4} | %{{
                                                  $_ | Add-Member -Name syn -MemberType noteproperty -Value (Get-Help $_) -PassThru
                                              }}

                                              """;

    /// <summary>
    /// Returns PowerShell module/snap-in command retrieval invocation string.
    /// </summary>
    /// <param name="moduleInfo">Module info.</param>
    /// <param name="commandTypes">A comma-separated string with applicable PowerShell command types, such as cmdlets, functions, etc.</param>
    /// <returns>PowerShell invocation string.</returns>
    public static String GetInvocationString(this IModuleInfo moduleInfo, IEnumerable<String> commandTypes) {
        String commandTypesString = String.Join(",", commandTypes);
        Object?[] args = new Object[5];
        args[1] = moduleInfo.Name;
        args[4] = commandTypesString;
        if (moduleInfo.ModuleClass is "Module" or "External") {
            args[0] = "Import-Module";
            args[2] = String.IsNullOrEmpty(moduleInfo.ModulePath)
                ? moduleInfo.Name
                : moduleInfo.ModulePath;
            args[3] = moduleInfo.ModuleClass == "External" || !moduleInfo.HasManifest
                ? null
                : " -RequiredVersion " + moduleInfo.Version;
            return String.Format(INVOCATION_STRING_TEMPLATE, args);
        }

        args[0] = "Add-PSSnapin";
        args[2] = args[3] = null;

        return String.Format(INVOCATION_STRING_TEMPLATE, args);
    }
}
