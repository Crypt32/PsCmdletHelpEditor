using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeKicker.BBCode;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Utils;

namespace PsCmdletHelpEditor.Core.Services.Formatters;

/// <summary>
/// Represents abstract class for HTML-based help source generator
/// </summary>
abstract class OutputProcessor : IHelpOutputFormatter {
    readonly Dictionary<String, String> _commandUrls = [];

    protected Boolean HandleNewLine { get; set; }
    protected Boolean EscapeHtml { get; set; }
    protected String LineBreak { get; set; } = Environment.NewLine;
    protected String NL { get; } = Environment.NewLine;

    /// <summary>
    /// Gets BB-code parser based on requested parser type.
    /// </summary>
    /// <param name="type">Requested parser type.</param>
    /// <returns>BB-code parser.</returns>
    protected abstract BBCodeParser GetParser(ParserType type);
    /// <summary>
    /// Generates Heading 1
    /// </summary>
    /// <param name="content">Heading content.</param>
    /// <returns>Markup.</returns>
    protected abstract String GenerateH1(String content);
    /// <summary>
    /// Generates Heading 2
    /// </summary>
    /// <param name="content">Heading content.</param>
    /// <returns>Markup.</returns>
    protected abstract String GenerateH2(String content);
    /// <summary>
    /// Generates Heading 3
    /// </summary>
    /// <param name="content">Heading content.</param>
    /// <returns>Markup.</returns>
    protected abstract String GenerateH3(String content);
    /// <summary>
    /// Generates preformatted code block.
    /// </summary>
    /// <param name="content">Code block content.</param>
    /// <param name="languageIdentifier"></param>
    /// <returns>Markup.</returns>
    protected abstract String GeneratePre(String content, String? languageIdentifier = null);
    /// <summary>
    /// Generates indented paragraph.
    /// </summary>
    /// <param name="content">Paragraph content.</param>
    /// <returns>Markup.</returns>
    protected abstract String GenerateParagraph(String content);
    /// <summary>
    /// Generates hyperlink.
    /// </summary>
    /// <param name="linkText">Link text.</param>
    /// <param name="linkUrl">Link URL.</param>
    /// <returns>Markup.</returns>
    protected abstract String GenerateHyperLink(String linkText, String linkUrl);
    /// <summary>
    /// Generates a bullet list out of encoded list content.
    /// </summary>
    /// <param name="listItems">Encoded list content.</param>
    /// <returns>Markup.</returns>
    protected abstract String GenerateList(String listItems);
    /// <summary>
    /// Generates bullet list item.
    /// </summary>
    /// <param name="content">List item.</param>
    /// <returns>Markup.</returns>
    protected abstract String GenerateListItem(String content);
    /// <summary>
    /// Generates cmdlet parameter table.
    /// </summary>
    /// <param name="param">Cmdlet parameter.</param>
    /// <returns>Markup.</returns>
    protected abstract String GenerateParamTable(IPsCommandParameter param);
    /// <summary>
    /// Generates warning alert panel.
    /// </summary>
    /// <param name="text">Text to wrap in warning alert.</param>
    /// <returns>Markup.</returns>
    protected abstract String GenerateWarningAlert(String text);

    String escape(String? content) {
        return EscapeHtml ? SecurityElement.Escape(content) : content;
    }
    String generateHtmlLink(String source, IEnumerable<IPsCommandInfo>? cmdlets) {
        if (cmdlets != null) {
            foreach (KeyValuePair<String, String> commandUrlEntry in _commandUrls
                         .Select(x => new {Entry = x, regex = new Regex(@"\b" + x.Key + @"\b") })
                         .Where(x => x.regex.IsMatch(source))
                         .Select(x => x.Entry)
            ) {
                source = source.Replace(commandUrlEntry.Key, $"[url={commandUrlEntry.Value}]{commandUrlEntry.Key}[/url]");
            }
        }

        if (HandleNewLine) {
            return source.Replace("\n\n", LineBreak + NL);
        }

        return source;
    }
    // populates a dictionary where key is command name and value is command URL, which is retrieved
    // either from command's metaweblog provider URL or from 'online version' related link.
    void preProcess(IReadOnlyList<IPsCommandInfo> commands) {
        foreach (IPsCommandInfo commandInfo in commands) {
            if (!String.IsNullOrWhiteSpace(commandInfo.URL)) {
                _commandUrls[commandInfo.Name] = commandInfo.URL!;

                return;
            }
            var regex = new Regex("online version:", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            IPsCommandRelatedLink? relatedLink = commandInfo.GetRelatedLinks()
                .FirstOrDefault(x => regex.IsMatch(x.LinkText ?? String.Empty) && !String.IsNullOrWhiteSpace(x.LinkUrl));
            if (relatedLink != null) {
                _commandUrls[commandInfo.Name] = relatedLink.LinkUrl!;
            }
        }
    }

    // generates pure encoded HTML string
    String generatePureHtml(IPsCommandInfo cmdlet, IReadOnlyList<IPsCommandInfo> cmdlets, StringBuilder SB, Boolean useSupports) {
        preProcess(cmdlets);
        IPsCommandGeneralDescription generalInfo = cmdlet.GetDescription();
        SB.Clear();
        BBCodeParser rules = GetParser(ParserType.Enhanced);
        htmlGenerateName(SB, cmdlet);
        htmlGenerateSynopsis(rules, SB, cmdlets, cmdlet);
        htmlGenerateSyntax(SB, cmdlet);
        htmlGenerateDescription(rules, SB, cmdlets, cmdlet);
        htmlGenerateParams(rules, SB, cmdlets, cmdlet);
        outGenerateTypes(rules, SB, generalInfo.InputType, generalInfo.InputUrl, generalInfo.InputTypeDescription, true);
        outGenerateTypes(rules, SB, generalInfo.ReturnType, generalInfo.ReturnUrl, generalInfo.ReturnTypeDescription, false);
        htmlGenerateNotes(rules, SB, cmdlet);
        htmlGenerateExamples(rules, SB, cmdlet);
        htmlGenerateRelatedLinks(rules, SB, cmdlets, cmdlet);
        if (useSupports) {
            htmlGenerateSupports(cmdlet.GetSupportInfo(), ref SB);
        }
        if (!String.IsNullOrEmpty(cmdlet.ExtraFooter)) {
            SB.Append(cmdlet.ExtraFooter);
        }
        return SB.ToString().TrimEnd();
    }
    void htmlGenerateName(StringBuilder SB, IPsCommandInfo cmdlet) {
        if (!String.IsNullOrEmpty(cmdlet.ExtraHeader)) {
            SB.Append(cmdlet.ExtraHeader);
        }
        SB.AppendLine(GenerateH1(escape(cmdlet.Name)));
    }
    void htmlGenerateSynopsis(BBCodeParser rules, StringBuilder SB, IReadOnlyList<IPsCommandInfo> cmdlets, IPsCommandInfo cmdlet) {
        SB.AppendLine(GenerateH2("Synopsis"));
        if (!String.IsNullOrEmpty(cmdlet.GetDescription().Synopsis)) {
            String str = rules.ToHtml(generateHtmlLink(cmdlet.GetDescription().Synopsis, cmdlets));
            SB.AppendLine(GenerateParagraph(str));
        }
    }
    void htmlGenerateSyntax(StringBuilder SB, IPsCommandInfo cmdlet) {
        SB.AppendLine(GenerateH2("Syntax"));
        foreach (String syntaxItem in cmdlet.GetSyntax()) {
            SB.AppendLine(GeneratePre(syntaxItem + " [<CommonParameters>]"));
        }
    }
    void htmlGenerateDescription(BBCodeParser rules, StringBuilder SB, IReadOnlyList<IPsCommandInfo> cmdlets, IPsCommandInfo cmdlet) {
        SB.AppendLine(GenerateH2("Description"));
        if (!String.IsNullOrEmpty(cmdlet.GetDescription().Description)) {
            String str = rules.ToHtml(generateHtmlLink(cmdlet.GetDescription().Description, cmdlets));
            SB.AppendLine(GenerateParagraph(str));
        }
    }
    void htmlGenerateParams(BBCodeParser rules, StringBuilder SB, IReadOnlyList<IPsCommandInfo> cmdlets, IPsCommandInfo cmdlet) {
        SB.AppendLine(GenerateH2("Parameters"));
        foreach (IPsCommandParameter param in cmdlet.GetParameters()) {
            String paramNameContent = $"-{escape(param.Name)} <em style=\"font-weight: 100;\">&lt;{escape(param.Type)}&gt;</em>";
            SB.AppendLine(GenerateH3(paramNameContent));
            if (!String.IsNullOrEmpty(param.Description)) {
                String str = rules.ToHtml(generateHtmlLink(param.Description, cmdlets));
                SB.AppendLine(GenerateParagraph(str));
            }
            SB.AppendLine(GenerateParamTable(param));
        }
        // Common parameters
        SB.AppendLine(GenerateH3("&lt;CommonParameters&gt;"));
        String link = GenerateHyperLink("https://go.microsoft.com/fwlink/?LinkID=113216", "https://go.microsoft.com/fwlink/?LinkID=113216");
        String br = HandleNewLine ? "<br/>" : String.Empty;
        SB.AppendLine(GenerateParagraph($"""
                                         This cmdlet supports the common parameters: Verbose, Debug,{br}
                                         ErrorAction, ErrorVariable, InformationAction, InformationVariable,{br}
                                         WarningAction, WarningVariable, OutBuffer, PipelineVariable and OutVariable.{br}
                                         For more information, see about_CommonParameters ({link})
                                         """));
    }
    void outGenerateTypes(BBCodeParser rules, StringBuilder SB, String? typesString, String? urlsString, String? String, Boolean input) {
        SB.AppendLine(input
            ? GenerateH2("Inputs")
            : GenerateH2("Outputs"));

        var types = new List<String>(typesString?.Split(';') ?? []);
        var urls = new List<String>(urlsString?.Split(';') ?? []);
        var descriptions = new List<String>(String?.Split(';') ?? []);

        for (Int32 index = 0; index < types.Count; index++) {
            if (String.IsNullOrWhiteSpace(types[index])) {
                continue;
            }
            if (index < urls.Count) {
                SB.AppendLine(String.IsNullOrEmpty(urls[index])
                    ? GenerateParagraph(rules.ToHtml(types[index]))
                    : GenerateParagraph(GenerateHyperLink(types[index], urls[index])));
            } else {
                SB.AppendLine(GenerateParagraph(rules.ToHtml(types[index])));
            }
            if (index < descriptions.Count) {
                String? description = descriptions[index];
                if (!String.IsNullOrWhiteSpace(description)) {
                    SB.AppendLine($"<p style=\"margin-left: 80px;\">{rules.ToHtml(description)}</p>");
                }
            }
        }
    }
    void htmlGenerateNotes(BBCodeParser rules, StringBuilder SB, IPsCommandInfo cmdlet) {
        SB.AppendLine(GenerateH2("Notes"));
        if (!String.IsNullOrEmpty(cmdlet.GetDescription().Notes)) {
            String str = rules.ToHtml(generateHtmlLink(cmdlet.GetDescription().Notes, null));
            SB.AppendLine(GenerateParagraph(str));
        }
    }
    void htmlGenerateExamples(BBCodeParser rules, StringBuilder SB, IPsCommandInfo cmdlet) {
        SB.AppendLine(GenerateH2("Examples"));
        for (Int32 index = 0; index < cmdlet.GetExamples().Count; index++) {
            IPsCommandExample? example = cmdlet.GetExamples()[index];
            String name = String.IsNullOrEmpty(example.Name)
                ? $"Example {index + 1}"
                : example.Name;
            SB.AppendLine(GenerateH3(escape(name)));
            if (!String.IsNullOrEmpty(example.Cmd)) {
                String cmd = !example.Cmd?.StartsWith("PS C:\\>") ?? false
                    ? $"PS C:\\> {example.Cmd}"
                    : example.Cmd;

                SB.AppendLine(GeneratePre(cmd, "PowerShell"));
            }

            if (!String.IsNullOrEmpty(example.Output)) {
                SB.AppendLine(GeneratePre(escape(example.Output)));
            }

            if (!String.IsNullOrEmpty(example.Description)) {
                String str = rules.ToHtml(example.Description);
                SB.AppendLine(GenerateParagraph(str));
            }
        }
    }
    void htmlGenerateRelatedLinks(BBCodeParser rules, StringBuilder SB, IReadOnlyList<IPsCommandInfo> cmdlets, IPsCommandInfo cmdlet) {
        SB.AppendLine(GenerateH2("Related links"));
        if (cmdlet.GetRelatedLinks().Count <= 0) {
            return;
        }

        var content = new StringBuilder();
        foreach (IPsCommandRelatedLink? link in cmdlet.GetRelatedLinks().Where(x => !String.Equals(x.LinkText, "online version:", StringComparison.OrdinalIgnoreCase))) {
            String linkEntry = String.IsNullOrEmpty(link.LinkUrl)
                ? rules.ToHtml(generateHtmlLink(link.LinkText, cmdlets))
                : GenerateHyperLink(link.LinkText, link.LinkUrl);
            linkEntry = GenerateListItem(linkEntry);
            content.AppendLine(linkEntry);
        }
        SB.AppendLine(GenerateList(content.ToString()));
    }
    void htmlGenerateSupports(IPsCommandSupportInfo supportInfo, ref StringBuilder SB) {
        var header = new StringBuilder();
        if (supportInfo.RequiresAD) {
            header.AppendLine(GenerateWarningAlert("This command is not available in non-domain environments"));
        }
        if (supportInfo.RequiresRSAT) {
            header.AppendLine(GenerateWarningAlert("This command requires installed Remote Server Administration Tools (RSAT)"));
        }
        SB = new StringBuilder(header.ToString() + SB);
        SB.AppendLine(GenerateH2("Minimum PowerShell version support"));
        String psVersion = GenerateListItem(supportInfo.PsVersion.GetAttributeOfType<DisplayAttribute>().Name);

        SB.AppendLine(GenerateList(psVersion));
        SB.AppendLine(GenerateH2("Operating System Support"));
        var osList = new StringBuilder();
        if ((supportInfo.WinOsVersion & WinOsVersionSupport.WinXP) == WinOsVersionSupport.WinXP) {
            osList.AppendLine(GenerateListItem("Windows XP"));
        }
        if ((supportInfo.WinOsVersion & WinOsVersionSupport.WinVista) == WinOsVersionSupport.WinVista) {
            osList.AppendLine(GenerateListItem("Windows Vista"));
        }
        if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win7) == WinOsVersionSupport.Win7) {
            osList.AppendLine(GenerateListItem("Windows 7"));
        }
        if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win8) == WinOsVersionSupport.Win8) {
            osList.AppendLine(GenerateListItem("Windows 8"));
        }
        if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win81) == WinOsVersionSupport.Win81) {
            osList.AppendLine(GenerateListItem("Windows 8.1"));
        }
        if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win10) == WinOsVersionSupport.Win10) {
            osList.AppendLine(GenerateListItem("Windows 10"));
        }
        if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win11) == WinOsVersionSupport.Win11) {
            osList.AppendLine(GenerateListItem("Windows 11"));
        }
        if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2003) == WinOsVersionSupport.Win2003) {
            osList.AppendLine(GenerateListItem("Windows Server 2003 all editions"));
        } else {
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2003Std) == WinOsVersionSupport.Win2003Std) {
                osList.AppendLine(GenerateListItem("Windows Server 2003 Standard"));
            }
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2003EE) == WinOsVersionSupport.Win2003EE) {
                osList.AppendLine(GenerateListItem("Windows Server 2003 Enterprise"));
            }
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2003DC) == WinOsVersionSupport.Win2003DC) {
                osList.AppendLine(GenerateListItem("Windows Server 2003 Datacenter"));
            }
        }
        if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2008) == WinOsVersionSupport.Win2008) {
            osList.AppendLine(GenerateListItem("Windows Server 2008 all editions"));
        } else {
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2008Std) == WinOsVersionSupport.Win2008Std) {
                osList.AppendLine(GenerateListItem("Windows Server 2008 Standard"));
            }
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2008EE) == WinOsVersionSupport.Win2008EE) {
                osList.AppendLine(GenerateListItem("Windows Server 2008 Enterprise"));
            }
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2008DC) == WinOsVersionSupport.Win2008DC) {
                osList.AppendLine(GenerateListItem("Windows Server 2008 Datacenter"));
            }
        }
        if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2008R2) == WinOsVersionSupport.Win2008R2) {
            osList.AppendLine(GenerateListItem("Windows Server 2008 R2 all editions"));
        } else {
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2008R2Std) == WinOsVersionSupport.Win2008R2Std) {
                osList.AppendLine(GenerateListItem("Windows Server 2008 R2 Standard"));
            }
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2008R2EE) == WinOsVersionSupport.Win2008R2EE) {
                osList.AppendLine(GenerateListItem("Windows Server 2008 R2 Enterprise"));
            }
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2008R2DC) == WinOsVersionSupport.Win2008R2DC) {
                osList.AppendLine(GenerateListItem("Windows Server 2008 R2 Datacenter"));
            }
        }
        if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2012) == WinOsVersionSupport.Win2012) {
            osList.AppendLine(GenerateListItem("Windows Server 2012 all editions"));
        } else {
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2012Std) == WinOsVersionSupport.Win2012Std) {
                osList.AppendLine(GenerateListItem("Windows Server 2012 Standard"));
            }
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2012DC) == WinOsVersionSupport.Win2012DC) {
                osList.AppendLine(GenerateListItem("Windows Server 2012 Datacenter"));
            }
        }
        if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2012R2) == WinOsVersionSupport.Win2012R2) {
            osList.AppendLine(GenerateListItem("Windows Server 2012 R2 all editions"));
        } else {
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2012R2Std) == WinOsVersionSupport.Win2012R2Std) {
                osList.AppendLine(GenerateListItem("Windows Server 2012 R2 Standard"));
            }
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2012R2DC) == WinOsVersionSupport.Win2012R2DC) {
                osList.AppendLine(GenerateListItem("Windows Server 2012 R2 Datacenter"));
            }
        }
        if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2016) == WinOsVersionSupport.Win2016) {
            osList.AppendLine(GenerateListItem("Windows Server 2016 all editions"));
        } else {
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2016Std) == WinOsVersionSupport.Win2016Std) {
                osList.AppendLine(GenerateListItem("Windows Server 2016 Standard"));
            }
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2016DC) == WinOsVersionSupport.Win2016DC) {
                osList.AppendLine(GenerateListItem("Windows Server 2016 Datacenter"));
            }
        }
        if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2019) == WinOsVersionSupport.Win2019) {
            osList.AppendLine(GenerateListItem("Windows Server 2019 all editions"));
        } else {
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2019Std) == WinOsVersionSupport.Win2019Std) {
                osList.AppendLine(GenerateListItem("Windows Server 2019 Standard"));
            }
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2019DC) == WinOsVersionSupport.Win2019DC) {
                osList.AppendLine(GenerateListItem("Windows Server 2019 Datacenter"));
            }
        }
        if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2022) == WinOsVersionSupport.Win2022) {
            osList.AppendLine(GenerateListItem("Windows Server 2022 all editions"));
        } else {
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2022Std) == WinOsVersionSupport.Win2022Std) {
                osList.AppendLine(GenerateListItem("Windows Server 2022 Standard"));
            }
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2022DC) == WinOsVersionSupport.Win2022DC) {
                osList.AppendLine(GenerateListItem("Windows Server 2022 Datacenter"));
            }
        }
        if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2025) == WinOsVersionSupport.Win2025) {
            osList.AppendLine(GenerateListItem("Windows Server 2025 all editions"));
        } else {
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2025Std) == WinOsVersionSupport.Win2025Std) {
                osList.AppendLine(GenerateListItem("Windows Server 2025 Standard"));
            }
            if ((supportInfo.WinOsVersion & WinOsVersionSupport.Win2025DC) == WinOsVersionSupport.Win2025DC) {
                osList.AppendLine(GenerateListItem("Windows Server 2025 Datacenter"));
            }
        }
        SB.AppendLine(GenerateList(osList.ToString()));
    }

    public String GenerateView(IPsCommandInfo cmdlet, IPsModuleProject moduleObject) {
        return generatePureHtml(cmdlet, moduleObject.GetCmdlets(), new StringBuilder(), moduleObject.UseSupports);
    }
    // generates HTML for web browser control
    public Task<String> GenerateViewAsync(IPsCommandInfo cmdlet, IPsModuleProject moduleObject) {
        return Task<String>.Run(() => GenerateView(cmdlet, moduleObject));
    }
}
