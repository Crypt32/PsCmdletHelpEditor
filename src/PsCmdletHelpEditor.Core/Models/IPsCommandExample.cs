using System;

namespace PsCmdletHelpEditor.Core.Models;

/// <summary>
/// Represents command example.
/// </summary>
public interface IPsCommandExample {
    /// <summary>
    /// Gets example display name. For example, "Example 1".
    /// </summary>
    String Name { get; }
    /// <summary>
    /// Gets example command or commands.
    /// </summary>
    String Cmd { get; }
    /// <summary>
    /// Gets optional example description.
    /// </summary>
    String? Description { get; }
    /// <summary>
    /// Gets optional command output.
    /// </summary>
    String? Output { get; }
}