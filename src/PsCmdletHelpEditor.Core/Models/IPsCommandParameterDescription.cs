using System;
using System.Collections.Generic;

namespace PsCmdletHelpEditor.Core.Models;

public interface IPsCommandParameterDescription {
    /// <summary>
    /// Gets cmdlet parameter name.
    /// </summary>
    String Name { get; set; }
    /// <summary>
    /// Gets parameter type name.
    /// </summary>
    String Type { get; set; }
    /// <summary>
    /// Gets parameter description.
    /// </summary>
    String Description { get; set; }
    /// <summary>
    /// Gets optional parameter default value.
    /// </summary>
    String? DefaultValue { get; set; }
    /// <summary>
    /// Gets parameter options.
    /// </summary>
    PsCommandParameterOption Options { get; }

    /// <summary>
    /// Gets a collection of parameter attributes (type names).
    /// </summary>
    ICollection<String> GetAttributes();
    /// <summary>
    /// Gets a collection of parameter aliases.
    /// </summary>
    ICollection<String> GetAliases();
}