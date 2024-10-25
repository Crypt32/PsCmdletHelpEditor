using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models;

/// <summary>
/// Represents PowerShell module or snap-in metadata information.
/// </summary>
public class PsModuleInfo {
    const String INVOCATION_STRING_TEMPLATE = """
                                              $Error.Clear()
                                              {0} -Name "{2}"{3} -ErrorAction Stop
                                              if ((Get-Module -Name {1}) -eq $null) {{
                                                  throw New-Object Exception $Error[0].Exception.Message
                                              }}
                                              Get-Command -Module {1} -CommandType {4} | %{{
                                                  $_ | Add-Member -Name syn -MemberType noteproperty -Value (get-help $_) -PassThru
                                              }}

                                              """;

    /// <summary>
    /// Gets or sets module name.
    /// </summary>
    public String Name { get; internal set; } = null!;
    /// <summary>
    /// Gets or sets module type.
    /// </summary>
    public ModuleType ModuleType { get; internal set; }
    /// <summary>
    /// Gets or sets module class.
    /// </summary>
    public String ModuleClass { get; internal set; } = null!;
    /// <summary>
    /// Gets or sets module version.
    /// </summary>
    public String Version { get; internal set; } = null!;
    /// <summary>
    /// Gets module description.
    /// </summary>
    public String? Description { get; internal set; }
    /// <summary>
    /// Indicates if module has manifest file (.psm1 and/or .psd1)
    /// </summary>
    public Boolean HasManifest { get; internal set; }
    /// <summary>
    /// Gets optional module file location.
    /// </summary>
    public String? ModulePath { get; set; }
    /// <summary>
    /// Gets value if PowerShell module is offline. If set to <c>true</c>, then module help content
    /// is available, however there is no such module installed on a system.
    /// </summary>
    public Boolean IsOffline { get; internal set; }

    public String GetInvocationString(IEnumerable<String> commandTypes) {
        String commandTypesString = String.Join(",", commandTypes);
        Object?[] args = new Object[5];
        args[1] = Name;
        args[4] = commandTypesString;
        if (ModuleClass is "Module" or "External") {
            args[0] = "Import-Module";
            args[2] = String.IsNullOrEmpty(ModulePath)
                ? Name
                : ModulePath;
            args[3] = ModuleClass == "External" || !HasManifest
                ? null
                : " -RequiredVersion " + Version;
            return String.Format(INVOCATION_STRING_TEMPLATE, args);
        }

        args[0] = "Add-PSSnapin";
        args[2] = args[3] = null;

        return String.Format(INVOCATION_STRING_TEMPLATE, args);
    }
}