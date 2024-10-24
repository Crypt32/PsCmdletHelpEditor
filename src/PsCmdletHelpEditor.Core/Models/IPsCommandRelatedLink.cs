using System;

namespace PsCmdletHelpEditor.Core.Models;

/// <summary>
/// Represents command related link.
/// </summary>
public interface IPsCommandRelatedLink {
    /// <summary>
    /// Gets link display text.
    /// </summary>
    String? LinkText { get; }
    /// <summary>
    /// Gets optional link URL.
    /// </summary>
    String? LinkUrl { get; }
}