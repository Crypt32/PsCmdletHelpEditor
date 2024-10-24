using System;
using System.Collections.Generic;

namespace PsCmdletHelpEditor.Core.Models;

/// <summary>
/// Gets command parameter set entry.
/// </summary>
public interface IPsCommandParameterSetInfo {
    /// <summary>
    /// Gets parameter set name.
    /// </summary>
    String Name { get; }

    /// <summary>
    /// Gets a collection of parameters used in this parameter set.
    /// </summary>
    /// <returns>A collection of parameters used in this parameter set.</returns>
    IReadOnlyList<String> GetParameters();


}