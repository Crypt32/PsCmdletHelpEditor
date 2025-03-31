using System;
using System.Security;
using System.Text;
using CodeKicker.BBCode;
using PsCmdletHelpEditor.Core.Models;

namespace PsCmdletHelpEditor.Core.Services.Formatters;
class HtmlProcessor : OutputProcessor {
    const String LINE_BREAK = "<br/>";

    protected override BBCodeParser GetParser(ParserType type) {
        switch (type) {
            case ParserType.Basic:
                return new BBCodeParser(ErrorMode.ErrorFree, null, [
                    new BBTag("b", "<strong>", "</strong>"),
                    new BBTag("i", "<i>", "</i>"),
                    new BBTag("u", "<u>", "</u>"),
                    new BBTag("s", "<s>", "</s>")
                ]);
            case ParserType.Enhanced:
                return new BBCodeParser(ErrorMode.ErrorFree, null, [
                    new BBTag("br", "<br />", String.Empty, true, false),
                    new BBTag("b", "<strong>", "</strong>"),
                    new BBTag("i", "<i>", "</i>"),
                    new BBTag("u", "<span style=\"text-decoration:underline;\">", "</span>"),
                    new BBTag("s", "<s>", "</s>"),
                    new BBTag("url", "<a href=\"${href}\">", "</a>", new BBAttribute("href", ""), new BBAttribute("href", "href")),
                    new BBTag("pre", "<pre>", "</pre>"),
                    new BBTag("quote", "<blockquote class=\"${class}\">", "</blockquote>", new BBAttribute("class", ""), new BBAttribute("class", "class")),
                    new BBTag("color", "<span style=\"color: ${color};\">","</span>", new BBAttribute("color", ""), new BBAttribute("color", "color"))
                ]);
            case ParserType.Clear:
                return new BBCodeParser(ErrorMode.ErrorFree, null, [
                    new BBTag("br", String.Empty, String.Empty),
                    new BBTag("b", String.Empty, String.Empty),
                    new BBTag("i", String.Empty, String.Empty),
                    new BBTag("u", String.Empty, String.Empty),
                    new BBTag("s", String.Empty, String.Empty),
                    new BBTag("url", "", "", new BBAttribute("",""),new BBAttribute("","")),
                    new BBTag("quote", "", "", new BBAttribute("",""),new BBAttribute("","")),
                    new BBTag("pre", "", "", new BBAttribute("",""),new BBAttribute("","")),
                    new BBTag("color", "", "", new BBAttribute("",""),new BBAttribute("",""))
                ]);
            default:
                return new BBCodeParser(ErrorMode.ErrorFree, null, [
                    new BBTag("b", String.Empty, String.Empty),
                    new BBTag("i", String.Empty, String.Empty),
                    new BBTag("u", String.Empty, String.Empty),
                    new BBTag("s", String.Empty, String.Empty)
                ]);
        }
    }
    protected override String GenerateH1(String content) {
        return "<h1>" + content + "</h1>";
    }
    protected override String GenerateH2(String content) {
        return "<h2>" + content + "</h2>";
    }
    protected override String GenerateH3(String content) {
        return "<h3>" + content + "</h3>";
    }
    protected override String GeneratePre(String content, String? languageIdentifier = null) {
        return "<pre>" + SecurityElement.Escape(content) + "</pre>";
    }
    protected override String GenerateParagraph(String content) {
        var sb = new StringBuilder();
        content = content.Trim().Replace("\r", null);
        String[] paragraphs = content.Split(["\n\n"], StringSplitOptions.RemoveEmptyEntries);
        foreach (String paragraph in paragraphs) {
            sb.Append("<p style =\"margin-left: 40px;\">" + paragraph.Replace("\n", LINE_BREAK) + "</p>");
        }
        return sb.ToString();
    }
    protected override String GenerateHyperLink(String linkText, String linkUrl) {
        return $"<a href=\"{linkUrl.Trim()}\">{linkText.Trim()}</a>";
    }
    protected override String GenerateList(String listItems) {
        return "<ul>" + NL + listItems + "</ul>";
    }
    protected override String GenerateListItem(String content) {
        return "<li>" + content + "</li>";
    }
    protected override String GenerateParamTable(IPsCommandParameter param) {
        return $"""
                <table style="margin-left: 40px;width: auto;" class="table table-condensed table-bordered">
                  <tbody>
                    <tr>
                      <td>Required?</td>
                      <td>{param.Mandatory}</td>
                    </tr>
                    <tr>
                      <td>Position?</td>
                      <td>{param.Position}</td>
                    </tr>
                    <tr>
                      <td>Default value</td>
                      <td>{param.DefaultValue}</td>
                    </tr>
                    <tr>
                      <td>Accept pipeline input?</td>
                      <td>{param.Pipeline}</td>
                    </tr>
                    <tr>
                      <td>Accept wildcard characters?</td>
                      <td>{param.Globbing}</td>
                    </tr>
                  </tbody>
                </table>
                """;
    }
    protected override String GenerateWarningAlert(String text) {
        if (String.IsNullOrEmpty(text)) {
            return String.Empty;
        }

        return $"<div class=\"alert alert-warning\">{text}</div>";
    }
}
