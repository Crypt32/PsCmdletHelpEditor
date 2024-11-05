using System;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models;

/// <summary>
/// Represents PowerShell module or snap-in metadata information.
/// </summary>
public class PsModuleInfo : IModuleInfo {
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
    /// Gets optional module manifest location.
    /// </summary>
    public String? ModulePath { get; set; }
    /// <summary>
    /// Gets value if PowerShell module is offline. If set to <c>true</c>, then module help content
    /// is available, however there is no such module installed on a system.
    /// </summary>
    public Boolean IsOffline { get; set; }
}