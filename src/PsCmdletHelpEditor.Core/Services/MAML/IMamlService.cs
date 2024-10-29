using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeKicker.BBCode;
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
    /// <param name="bbRules">BB-code parser.</param>
    /// <returns>XML string.</returns>
    Task<String> XmlGenerateHelp(ICollection<IPsCommandInfo> cmdlets, IProgress? pb, BBCodeParser bbRules);
}