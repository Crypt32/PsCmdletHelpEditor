using System;

namespace PsCmdletHelpEditor.Core.Models;
/// <summary>
/// Represents command online publish information. This is used when help content is published
/// using XML-RPC service.
/// </summary>
public interface IPsCommandPublishInfo {
    /// <summary>
    /// Determines if command is published online. If set to <c>false</c>, other members may be null.
    /// </summary>
    Boolean Publish { get; }
    /// <summary>
    /// Gets online article URL.
    /// </summary>
    String? URL { get; set; }
    /// <summary>
    /// Gets online article ID.
    /// </summary>
    String? ArticleIDString { get; set; }
}
