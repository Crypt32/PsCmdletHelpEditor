using System;

namespace PsCmdletHelpEditor.Core.Models;

/// <summary>
/// Stub interface for PS Processor
/// </summary>
public interface IModuleInfo {
    /// <summary>
    /// Gets or sets value if module is offline (not installed).
    /// </summary>
    Boolean IsOffline { get; set; }
    /// <summary>
    /// Returns PowerShell module/snap-in command retrieval invocation string.
    /// </summary>
    /// <param name="commandTypes">A comma-separated string with applicable PowerShell command types, such as cmdlets, functions, etc.</param>
    /// <returns>PowerShell invocation string.</returns>
    String GetInvocationString(String commandTypes);
}