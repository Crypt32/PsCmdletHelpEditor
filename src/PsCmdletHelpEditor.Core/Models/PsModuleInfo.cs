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


}