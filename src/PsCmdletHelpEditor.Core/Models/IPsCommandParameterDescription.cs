using System;
using System.Collections.Generic;

namespace PsCmdletHelpEditor.Core.Models;

public interface IPsCommandParameterDescription {
    /// <summary>
    /// Gets cmdlet parameter name.
    /// </summary>
    String Name { get; }
    /// <summary>
    /// Gets parameter type name.
    /// </summary>
    String Type { get; }
    /// <summary>
    /// Gets parameter description.
    /// </summary>
    String? Description { get; }
    /// <summary>
    /// Gets optional parameter default value.
    /// </summary>
    String? DefaultValue { get; }
    /// <summary>
    /// Gets parameter position. Can be a number or string if parameter is named.
    /// </summary>
    String Position { get; }
    /// <summary>
    /// Gets parameter options.
    /// </summary>
    PsCommandParameterOption Options { get; }

    /// <summary>
    /// Gets a collection of parameter attributes (type names).
    /// </summary>
    IReadOnlyList<String> GetAttributes();
    /// <summary>
    /// Gets a collection of parameter aliases.
    /// </summary>
    IReadOnlyList<String> GetAliases();
}