using System;

namespace PsCmdletHelpEditor.Core.Models;

/// <summary>
/// Represents command general help information.
/// </summary>
public interface IPsCommandGeneralDescription {
    /// <summary>
    /// Gets command synopsis, or short description.
    /// </summary>
    String Synopsis { get; }
    /// <summary>
    /// Gets command detailed description.
    /// </summary>
    String Description { get; }
    /// <summary>
    /// Gets optional notes.
    /// </summary>
    String? Notes { get; }
    /// <summary>
    /// Gets semicolon delimited optional input types.
    /// </summary>
    String InputType { get; }
    /// <summary>
    /// Gets semicolon delimited optional input type URLs.
    /// </summary>
    String InputUrl { get; }
    /// <summary>
    /// Gets semicolon delimited optional return types.
    /// </summary>
    String ReturnType { get; }
    /// <summary>
    /// Gets semicolon delimited optional return type URLs.
    /// </summary>
    String ReturnUrl { get; }
    /// <summary>
    /// Gets semicolon delimited input type descriptions.
    /// </summary>
    String InputTypeDescription { get; }
    /// <summary>
    /// Gets semicolon delimited return type descriptions.
    /// </summary>
    String ReturnTypeDescription { get; }
}