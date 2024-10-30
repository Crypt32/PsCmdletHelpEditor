namespace PsCmdletHelpEditor.Core.Services.Formatters;

/// <summary>
/// Represents factory class for output formatters.
/// </summary>
public static class OutputFormatterFactory {
    /// <summary>
    /// Gets a HTML help formatter.
    /// </summary>
    /// <returns>HTML help formatter.</returns>
    public static IHelpOutputFormatter GetHtmlFormatter() {
        return new HtmlProcessor();
    }
    /// <summary>
    /// Gets a Markdown help formatter.
    /// </summary>
    /// <returns>Markdown help formatter.</returns>
    public static IHelpOutputFormatter GetMarkdownFormatter() {
        return new MarkdownProcessor();
    }
}