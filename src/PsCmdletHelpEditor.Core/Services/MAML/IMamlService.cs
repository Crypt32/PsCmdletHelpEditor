using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PsCmdletHelpEditor.Core.Models;

namespace PsCmdletHelpEditor.Core.Services.MAML;

/// <summary>
/// Represents PowerShell MAML help service.
/// </summary>
public interface IMamlService {
    /// <summary>
    /// Generates a single XML help file using MAML.
    /// </summary>
    /// <param name="cmdlets">A collection of commands to export.</param>
    /// <param name="pb">Optional progress bar.</param>
    /// <returns>XML string.</returns>
    Task<String> XmlGenerateHelp(ICollection<IPsCommandInfo> cmdlets, IProgress? pb);
}