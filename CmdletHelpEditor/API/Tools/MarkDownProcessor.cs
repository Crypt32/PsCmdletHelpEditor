using System;
using CmdletHelpEditor.API.Models;
using CodeKicker.BBCode;

namespace CmdletHelpEditor.API.Tools;

/// <summary>
/// Represents Markdown source markup generator.
/// </summary>
[Obsolete("Obsolete", true)]
class MarkDownProcessor : OutputProcessor {
    static readonly Char[] _escapeChars = new[] {
            '#', '{', '}', '[', ']', '<', '>', '*', '+', '-', '|', '\\', '`', '_',
            '.'
        };

    public override BBCodeParser GetParser(ParserType type) {
        switch (type) {
            case ParserType.Basic:
                return new BBCodeParser(ErrorMode.ErrorFree, null, [
                    new BBTag("b", "**", "**"),
                    new BBTag("i", "_", "_"),
                    new BBTag("u", "<u>", "</u>"),
                    new BBTag("s", "~", "~")
                ]);
            case ParserType.Enhanced:
                return new BBCodeParser(ErrorMode.ErrorFree, null, [
                    new BBTag("br", "<br />", String.Empty, true, false),
                    new BBTag("b", "**", "**"),
                    new BBTag("i", "_", "_"),
                    new BBTag("u", "<u>", "</u>"),
                    new BBTag("s", "~", "~"),
                    new BBTag("url", "<a href=\"${href}\">", "</a>", new BBAttribute("href", ""), new BBAttribute("href", "href")),
                    new BBTag("pre", $"```{NL}", $"{NL}```"),
                    new BBTag("quote", $"{NL}> ", "", new BBAttribute("class", ""), new BBAttribute("class", "class")),
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
        return "# " + content;
    }
    protected override String GenerateH2(String content) {
        return NL + "## " + content;
    }
    protected override String GenerateH3(String content) {
        return NL + "### " + content;
    }
    protected override String GeneratePre(String content) {
        return $@"
```
{content}
```
";
    }
    protected override String GenerateParagraph(String content) {
        return NL + content + NL;// $"<p style=\"margin-left: 40px;\">{content}</p>";
    }
    protected override String GenerateHyperLink(String linkText, String linkUrl) {
        return $"[{linkText}]({linkUrl})";
    }
    protected override String GenerateList(String listItems) {
        return listItems;
    }
    protected override String GenerateListItem(String content) {
        return "- " + content;
    }
    protected override String GenerateParamTable(PsCommandParameterVM param) {
        return $@"
Property|Value
-|-
Required?|{param.Mandatory}
Position?|{param.Position}
Default value|{param.DefaultValue ?? "&nbsp;"}
Accept pipeline input?|{param.PipelineInput}
Accept wildcard characters?|{param.Globbing}
";
    }
}
