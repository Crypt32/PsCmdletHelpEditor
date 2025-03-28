namespace PsCmdletHelpEditor.Core.Services.Formatters;

/// <summary>
/// Represents BB-code parser type.
/// </summary>
enum ParserType {
    /// <summary>
    /// Supports basic BB-code rules. This includes the following tags: <c>b</c>, <c>i</c>, <c>u</c> and <c>s</c>.
    /// </summary>
    Basic,
    /// <summary>
    /// Supports extended BB-code rules. In addition to basic, this includes the following tags:
    /// <c>br</c>, <c>url</c>, <c>pre</c>, <c>quote</c> and <c>color</c>.
    /// </summary>
    Enhanced,
    /// <summary>
    /// Clear BB-code rules. This parser will convert BB-formatted text to plain text by removing all BB-code tags.
    /// </summary>
    Clear
}