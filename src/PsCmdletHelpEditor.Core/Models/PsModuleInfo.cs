using System;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models;

/// <summary>
/// Represents PowerShell module or snap-in metadata information.
/// </summary>
public class PsModuleInfo {
    /// <summary>
    /// Gets or sets module name.
    /// </summary>
    public String Name { get; set; } = null!;
    /// <summary>
    /// Gets or sets module type.
    /// </summary>
    public ModuleType ModuleType { get; set; }
    /// <summary>
    /// Gets or sets module class.
    /// </summary>
    public String ModuleClass { get; set; } = null!;
    /// <summary>
    /// Gets or sets module version.
    /// </summary>
    public String Version { get; set; } = null!;
    /// <summary>
    /// Gets module description.
    /// </summary>
    public String? Description { get; set; }
    /// <summary>
    /// Indicates if module has manifest file (.psm1 and/or .psd1)
    /// </summary>
    public Boolean HasManifest { get; set; }
    /// <summary>
    /// Gets optional module file location.
    /// </summary>
    public String? ModulePath { get; set; }
}