using System;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models;

/// <summary>
/// Stub interface for PS Processor
/// </summary>
public interface IModuleInfo {
    /// <summary>
    /// Gets or sets module name.
    /// </summary>
    String Name { get; }
    /// <summary>
    /// Gets or sets module type.
    /// </summary>
    ModuleType ModuleType { get; }
    /// <summary>
    /// Gets or sets module class.
    /// </summary>
    String ModuleClass { get; }
    /// <summary>
    /// Gets or sets module version.
    /// </summary>
    String Version { get; }
    /// <summary>
    /// Gets module description.
    /// </summary>
    String? Description { get; }
    /// <summary>
    /// Indicates if module has manifest file (.psm1 and/or .psd1)
    /// </summary>
    Boolean HasManifest { get; }
    /// <summary>
    /// Gets optional module file location.
    /// </summary>
    String? ModulePath { get; set; }
    /// <summary>
    /// Gets or sets value if module is offline (not installed).
    /// </summary>
    Boolean IsOffline { get; set; }
}