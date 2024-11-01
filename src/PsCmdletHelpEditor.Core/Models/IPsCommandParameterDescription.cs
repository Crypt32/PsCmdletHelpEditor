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
    /// Indicates if parameter accepts a collection of values.
    /// </summary>
    Boolean AcceptsArray { get; }
    /// <summary>
    /// Indicates if parameter is mandatory.
    /// </summary>
    Boolean Mandatory { get; }
    /// <summary>
    /// Indicates if parameter is dynamic.
    /// </summary>
    Boolean Dynamic { get; }
    /// <summary>
    /// Indicates if parameter accepts value from remaining args.
    /// </summary>
    Boolean RemainingArgs { get; }
    /// <summary>
    /// Indicates if parameter accepts value from pipeline.
    /// </summary>
    Boolean Pipeline { get; }
    /// <summary>
    /// Indicates if parameter accepts value from pipeline by matching property name
    /// (when property name in pipeline object match parameter name).
    /// </summary>
    Boolean PipelinePropertyName { get; }
    /// <summary>
    /// Indicates if parameter accepts wildcards.
    /// </summary>
    Boolean Globbing { get; }
    /// <summary>
    /// Indicates if parameter is positional.
    /// </summary>
    Boolean Positional { get; }
    /// <summary>
    /// Gets parameter position. Can be a number or string if parameter is named.
    /// </summary>
    String Position { get; }
    /// <summary>
    /// Gets optional parameter default value.
    /// </summary>
    String? DefaultValue { get; }
    /// <summary>
    /// Determines if parameter is orphaned. When set to <c>true</c>, then parameter exists in project/MAML help,
    /// however online command does not have this parameter.
    /// </summary>
    Boolean IsOrphaned { get; }

    /// <summary>
    /// Gets a collection of parameter attributes (type names).
    /// </summary>
    IReadOnlyList<String> GetAttributes();
    /// <summary>
    /// Gets a collection of parameter aliases.
    /// </summary>
    IReadOnlyList<String> GetAliases();
}