using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PsCmdletHelpEditor.Core.Models;

namespace PsCmdletHelpEditor.Core.Services;

public interface IPowerShellProcessor {
    /// <summary>
    /// Gets current PowerShell version.
    /// </summary>
    Int32? PsVersion { get; }
    
    /// <summary>
    /// Gets currently installed PowerShell version asynchronously.
    /// </summary>
    /// <returns>PowerShell version. Can be null if PowerShell version retrieval failed.</returns>
    Task<Int32?> GetPsVersionAsync();
    /// <summary>
    /// Gets currently installed PowerShell version synchronously.
    /// </summary>
    /// <returns>PowerShell version. Can be null if PowerShell version retrieval failed.</returns>
    Int32? GetPsVersion();
    /// <summary>
    /// Enumerates installed PowerShell modules and snap-ins asynchronously.
    /// </summary>
    /// <param name="force">Specifies if previously cached modules must be ignored and reloaded.</param>
    /// <returns>A collection of installed PowerShell module and snap-in metadata.</returns>
    Task<IEnumerable<PsModuleInfo>> EnumModulesAsync(Boolean force);
    /// <summary>
    /// Enumerates installed PowerShell modules and snap-ins synchronously.
    /// </summary>
    /// <param name="force">Specifies if previously cached modules must be ignored and reloaded.</param>
    /// <returns>A collection of installed PowerShell module and snap-in metadata.</returns>
    IEnumerable<PsModuleInfo> EnumModules(Boolean force);
    /// <summary>
    /// Retrieves PowerShell module metadata from module manifest file asynchronously.
    /// </summary>
    /// <param name="path">Path to PowerShell module manifest file (.psm1 or .psd1).</param>
    /// <returns>PowerShell module metadata.</returns>
    Task<PsModuleInfo> GetModuleInfoFromFileAsync(String path);
    /// <summary>
    /// Retrieves PowerShell module metadata from module manifest file synchronously.
    /// </summary>
    /// <param name="path">Path to PowerShell module manifest file (.psm1 or .psd1).</param>
    /// <returns>PowerShell module metadata.</returns>
    PsModuleInfo GetModuleInfoFromFile(String path);
}