using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CmdletHelpEditor.API.Models;
using CodeKicker.BBCode;

namespace CmdletHelpEditor.API.Tools {
    static class HtmlProcessor {
        static readonly String _nl = Environment.NewLine;
        public static BBCodeParser GetParser(ParserType type) {
            switch (type) {
                case ParserType.Basic:
                    return new BBCodeParser(ErrorMode.ErrorFree, null, new[] {
                        new BBTag("b", "<strong>", "</strong>"),
                        new BBTag("i", "<span style=\"font-style:italic;\">", "</span>"),
                        new BBTag("u", "<span style=\"text-decoration:underline;\">", "</span>"),
                        new BBTag("s", "<strike>", "</strike>")
                    });
                case ParserType.Enhanced:
                    return new BBCodeParser(ErrorMode.ErrorFree, null, new[] {
                        new BBTag("br", "<br />", String.Empty, true, false),
                        new BBTag("b", "<strong>", "</strong>"),
                        new BBTag("i", "<span style=\"font-style:italic;\">", "</span>"),
                        new BBTag("u", "<span style=\"text-decoration:underline;\">", "</span>"),
                        new BBTag("s", "<strike>", "</strike>"),
                        new BBTag("url", "<a href=\"${href}\">", "</a>", new BBAttribute("href", ""), new BBAttribute("href", "href")),
                        new BBTag("pre", "<pre>", "</pre>"),
                        new BBTag("quote", "<blockquote class=\"${class}\">", "</blockquote>", new BBAttribute("class", ""), new BBAttribute("class", "class")),
                        //new BBTag("pre", "<pre class=\"${class}\">", "</pre>", new BBAttribute("class", ""), new BBAttribute("class", "class")),
                        //new BBTag("pre", "<pre class=\"${class}\">", "</pre>", new BBAttribute("class", ""), new BBAttribute("class", "class")),
                        new BBTag("color", "<span style=\"color: ${color};\">","</span>", new BBAttribute("color", ""), new BBAttribute("color", "color"))
                    });
                case ParserType.Clear:
                    return new BBCodeParser(ErrorMode.ErrorFree, null, new[] {
                        new BBTag("br", String.Empty, String.Empty),
                        new BBTag("b", String.Empty, String.Empty),
                        new BBTag("i", String.Empty, String.Empty),
                        new BBTag("u", String.Empty, String.Empty),
                        new BBTag("s", String.Empty, String.Empty),
                        new BBTag("url", "", "", new BBAttribute("",""),new BBAttribute("","")),
                        new BBTag("quote", "", "", new BBAttribute("",""),new BBAttribute("","")),
                        new BBTag("pre", "", "", new BBAttribute("",""),new BBAttribute("","")),
                        new BBTag("color", "", "", new BBAttribute("",""),new BBAttribute("",""))
                    });
                default:
                    return new BBCodeParser(ErrorMode.ErrorFree, null, new[] {
                        new BBTag("b", String.Empty, String.Empty),
                        new BBTag("i", String.Empty, String.Empty),
                        new BBTag("u", String.Empty, String.Empty),
                        new BBTag("s", String.Empty, String.Empty)
                    });
            }
        }

        static String addIndentedParagraphText(String paragraph) {
            return $"<p style=\"margin-left: 40px;\">{paragraph}</p>";
        }
        static String generateHtmlLink(String source, IEnumerable<CmdletObject> cmdlets) {
            if (cmdlets != null) {
                foreach (CmdletObject cmdlet in from cmdlet in cmdlets let regex = new Regex(@"\b" + cmdlet.Name + @"\b") where regex.IsMatch(source) == !String.IsNullOrEmpty(cmdlet.URL) select cmdlet) {
                    source = source.Replace(cmdlet.Name, $"[url={cmdlet.URL}]{cmdlet.Name}[/url]");
                }
            }
            return source.Replace("\n", "[br]" + _nl);
        }
        // generates pure encoded HTML string
        static String generatePureHtml(CmdletObject cmdlet, IReadOnlyList<CmdletObject> cmdlets, StringBuilder SB, Boolean useSupports) {
            SB.Clear();
            BBCodeParser rules = GetParser(ParserType.Enhanced);
            htmlGenerateName(SB, cmdlet);
            htmlGenerateSynopsis(rules, SB, cmdlets, cmdlet);
            htmlGenerateSyntax(SB, cmdlet);
            htmlGenerateDescription(rules, SB, cmdlets, cmdlet);
            htmlGenerateParams(rules, SB, cmdlets, cmdlet);
            htmlGenerateInputTypes(rules, SB, cmdlet);
            htmlGenerateReturnTypes(rules, SB, cmdlet);
            htmlGenerateNotes(rules, SB, cmdlet);
            htmlGenerateExamples(rules, SB, cmdlet);
            htmlGenerateRelatedLinks(rules, SB, cmdlets, cmdlet);
            if (useSupports) { htmlGenerateSupports(cmdlet, ref SB); }
            if (!String.IsNullOrEmpty(cmdlet.ExtraFooter)) {
                SB.Append(cmdlet.ExtraFooter);
            }
            return SB.ToString();
        }
        static void htmlGenerateName(StringBuilder SB, CmdletObject cmdlet) {
            if (!String.IsNullOrEmpty(cmdlet.ExtraHeader)) {
                SB.Append(cmdlet.ExtraHeader);
            }
            SB.AppendLine($"<h1 style=\"text-align: center;\">{SecurityElement.Escape(cmdlet.Name)}</h1>");
        }
        static void htmlGenerateSynopsis(BBCodeParser rules, StringBuilder SB, IReadOnlyList<CmdletObject> cmdlets, CmdletObject cmdlet) {
            SB.AppendLine("<h2>Synopsis</h2>");
            if (!String.IsNullOrEmpty(cmdlet.GeneralHelp.Synopsis)) {
                String str = rules.ToHtml(generateHtmlLink(cmdlet.GeneralHelp.Synopsis, cmdlets));
                SB.AppendLine(addIndentedParagraphText(str));
            }
        }
        static void htmlGenerateSyntax(StringBuilder SB, CmdletObject cmdlet) {
            SB.AppendLine("<h2>Syntax</h2>");
            SB.Append("<pre style=\"margin-left: 40px;\">");
            foreach (String syntaxItem in cmdlet.Syntax) {
                SB.AppendLine(SecurityElement.Escape(syntaxItem) + " [&lt;CommonParameters&gt;]");
                SB.AppendLine(String.Empty);
            }
            SB.AppendLine("</pre>");
        }
        static void htmlGenerateDescription(BBCodeParser rules, StringBuilder SB, IReadOnlyList<CmdletObject> cmdlets, CmdletObject cmdlet) {
            SB.AppendLine("<h2>Description</h2>");
            if (!String.IsNullOrEmpty(cmdlet.GeneralHelp.Description)) {
                String str = rules.ToHtml(generateHtmlLink(cmdlet.GeneralHelp.Description, cmdlets));
                SB.AppendLine(addIndentedParagraphText(str));
            }
        }
        static void htmlGenerateParams(BBCodeParser rules, StringBuilder SB, IReadOnlyList<CmdletObject> cmdlets, CmdletObject cmdlet) {
            SB.AppendLine("<h2>Parameters</h2>");
            foreach (ParameterDescription param in cmdlet.Parameters) {
                SB.AppendLine($"<h3>-{SecurityElement.Escape(param.Name)} <em style=\"font-weight: 100;\">&lt;{SecurityElement.Escape(param.Type)}&gt;</em></h3>");
                if (!String.IsNullOrEmpty(param.Description)) {
                    String str = rules.ToHtml(generateHtmlLink(param.Description, cmdlets));
                    SB.AppendLine(addIndentedParagraphText(str));
                }
                SB.AppendLine($@"
<table style=""margin-left: 40px;width: auto;"" class=""table table-condensed table-bordered"">
    <tbody>
        <tr>
            <td>Required?</td>
            <td>{Convert.ToString(param.Mandatory)}</td>
        </tr>
        <tr>
            <td>Position?</td>
            <td>{param.Position}</td>
        </tr>
        <tr>
            <td>Default value</td>
            <td>{param.DefaultValue ?? "&nbsp;"}</td>
        </tr>
        <tr>
            <td>Accept pipeline input?</td>
            <td>{param.PipelineInput}</td>
        </tr>
        <tr>
            <td>Accept wildcard characters?</td>
            <td>{Convert.ToString(param.Globbing)}</td>
        </tr>
    </tbody>
</table>
"
);
            }
            // Common parameters
            SB.AppendLine(@"
<h3>&lt;CommonParameters&gt;</h3>
<p style=""margin-left: 40px;"">This cmdlet supports the common parameters: Verbose, Debug,<br />
ErrorAction, ErrorVariable, InformationAction, InformationVariable,<br />
WarningAction, WarningVariable, OutBuffer, PipelineVariable and OutVariable.<br />
For more information, see about_CommonParameters (<a href=""https://go.microsoft.com/fwlink/?LinkID=113216"">https://go.microsoft.com/fwlink/?LinkID=113216</a>).</p>
");
            
        }
        static void htmlGenerateInputTypes(BBCodeParser rules, StringBuilder SB, CmdletObject cmdlet) {
            SB.AppendLine("<h2>Inputs</h2>");
            List<String> inputTypes = new List<String>(cmdlet.GeneralHelp.InputType.Split(';'));
            List<String> inputUrls = new List<String>(cmdlet.GeneralHelp.InputUrl.Split(';'));
            List<String> inputDescription = new List<String>(cmdlet.GeneralHelp.InputTypeDescription.Split(';'));
            for (Int32 index = 0; index < inputTypes.Count; index++) {
                if (index < inputUrls.Count) {
                    if (String.IsNullOrEmpty(inputUrls[index])) {
                        SB.AppendLine(addIndentedParagraphText(rules.ToHtml(inputTypes[index])));
                    } else {
                        SB.AppendLine(addIndentedParagraphText($"<a href=\"{inputUrls[index]}\">{rules.ToHtml(inputTypes[index])}</a>"));
                    }
                } else {
                    SB.AppendLine(addIndentedParagraphText(rules.ToHtml(inputTypes[index])));
                }
                if (index < inputDescription.Count) {
                    SB.AppendLine($"<p style=\"margin-left: 80px;\">{rules.ToHtml(inputDescription[index])}</p>");
                }
            }
        }
        static void htmlGenerateReturnTypes(BBCodeParser rules, StringBuilder SB, CmdletObject cmdlet) {
            SB.Append("<h2>Outputs</h2>" + _nl);
            List<String> returnTypes = new List<String>(cmdlet.GeneralHelp.ReturnType.Split(';'));
            List<String> returnUrls = new List<String>(cmdlet.GeneralHelp.ReturnUrl.Split(';'));
            List<String> returnDescription = new List<String>(cmdlet.GeneralHelp.ReturnTypeDescription.Split(';'));
            for (Int32 index = 0; index < returnTypes.Count; index++) {
                if (index < returnUrls.Count) {
                    SB.AppendLine(String.IsNullOrEmpty(returnUrls[index])
                        ? addIndentedParagraphText(rules.ToHtml(returnTypes[index]))
                        : addIndentedParagraphText($"<a href=\"{returnUrls[index]}\">{rules.ToHtml(returnTypes[index])}</a>"));
                } else {
                    SB.AppendLine(addIndentedParagraphText(rules.ToHtml(returnTypes[index])));
                }
                if (index < returnDescription.Count) {
                    SB.AppendLine($"<p style=\"margin-left: 80px;\">{rules.ToHtml(returnDescription[index])}</p>");
                }
            }
        }
        static void htmlGenerateNotes(BBCodeParser rules, StringBuilder SB, CmdletObject cmdlet) {
            SB.AppendLine("<h2>Notes</h2>");
            if (!String.IsNullOrEmpty(cmdlet.GeneralHelp.Notes)) {
                String str = rules.ToHtml(generateHtmlLink(cmdlet.GeneralHelp.Notes, null));
                SB.AppendLine(addIndentedParagraphText(str));
            }
        }
        static void htmlGenerateExamples(BBCodeParser rules, StringBuilder SB, CmdletObject cmdlet) {
            SB.AppendLine("<h2>Examples</h2>");
            for (Int32 index = 0; index < cmdlet.Examples.Count; index++) {
                Example example = cmdlet.Examples[index];
                String name = String.IsNullOrEmpty(example.Name)
                    ? $"Example {index + 1}"
                    : example.Name;
                SB.AppendLine($"<h3>{SecurityElement.Escape(name)}</h3>");
                if (!String.IsNullOrEmpty(example.Cmd)) {
                    String cmd = !example.Cmd.StartsWith("PS C:\\>")
                        ? $"PS C:\\> {example.Cmd}"
                        : example.Cmd;

                    SB.AppendLine($"<pre style=\"margin-left: 40px;\">{SecurityElement.Escape(cmd)}</pre>");
                }

                if (!String.IsNullOrEmpty(example.Output)) {
                    SB.AppendLine($"<pre style=\"margin-left: 40px;\">{SecurityElement.Escape(example.Output)}</pre>");
                }

                if (!String.IsNullOrEmpty(example.Description)) {
                    String str = rules.ToHtml(example.Description);
                    SB.AppendLine(addIndentedParagraphText(str));
                }
            }
        }
        static void htmlGenerateRelatedLinks(BBCodeParser rules, StringBuilder SB, IReadOnlyList<CmdletObject> cmdlets, CmdletObject cmdlet) {
            SB.AppendLine("<h2>Related links</h2>");
            if (cmdlet.RelatedLinks.Count <= 0) {
                return;
            }

            SB.AppendLine("<p style=\"margin-left: 40px;\">");
            foreach (RelatedLink link in cmdlet.RelatedLinks.Where(x => !x.LinkText.Equals("online version:", StringComparison.OrdinalIgnoreCase))) {
                SB.Append("    " + rules.ToHtml(generateHtmlLink(link.LinkText, cmdlets)));
                if (!String.IsNullOrEmpty(link.LinkUrl)) {
                    SB.Append($"    <a href=\"{link.LinkUrl}\">{link.LinkUrl}</a>");
                }
                SB.AppendLine("<br />");
            }
            SB.AppendLine("</p>");
        }
        static void htmlGenerateSupports(CmdletObject cmdlet, ref StringBuilder SB) {
            String currentHtml = String.Empty;
            if (cmdlet.SupportInformation.ADChecked) {
                currentHtml += "<div class=\"alert alert-warning\">This command is not available in non-domain environments</div>" + _nl;
            }
            if (cmdlet.SupportInformation.RsatChecked) {
                currentHtml += "<div class=\"alert alert-warning\">This command requires installed Remote Server Administration Tools (RSAT)</div>" + _nl;
            }
            SB = new StringBuilder(currentHtml + SB);
            SB.AppendLine("<h2>Minimum PowerShell version support</h2>" + _nl);
            String psver;
            if (cmdlet.SupportInformation.Ps2Checked) {
                psver = "PowerShell 2.0";
            } else if (cmdlet.SupportInformation.Ps3Checked) {
                psver = "PowerShell 3.0";
            } else if (cmdlet.SupportInformation.Ps4Checked) {
                psver = "PowerShell 4.0";
            } else if (cmdlet.SupportInformation.Ps5Checked) {
                psver = "PowerShell 5.0";
            } else if (cmdlet.SupportInformation.Ps60Checked) {
                psver = "PowerShell 6.0";
            } else if (cmdlet.SupportInformation.Ps61Checked) {
                psver = "PowerShell 6.1";
            } else {
                psver = "Any";
            }
            SB.AppendLine($"<ul><li>{psver}</li></ul>");
            SB.AppendLine("<h2>Operating System Support</h2>");
            SB.AppendLine("<ul>");
            if (cmdlet.SupportInformation.WinXpChecked) {
                SB.AppendLine("	<li>Windows XP</li>");
            }
            if (cmdlet.SupportInformation.WinVistaChecked) {
                SB.AppendLine("	<li>Windows Vista</li>");
            }
            if (cmdlet.SupportInformation.Win7Checked) {
                SB.AppendLine("	<li>Windows 7</li>");
            }
            if (cmdlet.SupportInformation.Win8Checked) {
                SB.AppendLine("	<li>Windows 8</li>");
            }
            if (cmdlet.SupportInformation.Win81Checked) {
                SB.AppendLine("	<li>Windows 8.1</li>");
            }
            if (cmdlet.SupportInformation.Win10Checked) {
                SB.AppendLine("	<li>Windows 10</li>");
            }
            if (cmdlet.SupportInformation.Win2003Checked) {
                SB.AppendLine("	<li>Windows Server 2003 all editions</li>");
            } else {
                if (cmdlet.SupportInformation.Win2003StdChecked) {
                    SB.AppendLine("	<li>Windows Server 2003 Standard</li>");
                }
                if (cmdlet.SupportInformation.Win2003EEChecked) {
                    SB.AppendLine("	<li>Windows Server 2003 Enterprise</li>");
                }
                if (cmdlet.SupportInformation.Win2003DCChecked) {
                    SB.AppendLine("	<li>Windows Server 2003 Datacenter</li>");
                }
            }
            if (cmdlet.SupportInformation.Win2008Checked) {
                SB.AppendLine("	<li>Windows Server 2008 all editions</li>");
            } else {
                if (cmdlet.SupportInformation.Win2008StdChecked) {
                    SB.AppendLine("	<li>Windows Server 2008 Standard</li>");
                }
                if (cmdlet.SupportInformation.Win2008EEChecked) {
                    SB.AppendLine("	<li>Windows Server 2008 Enterprise</li>");
                }
                if (cmdlet.SupportInformation.Win2008DCChecked) {
                    SB.AppendLine("	<li>Windows Server 2008 Datacenter</li>");
                }
            }
            if (cmdlet.SupportInformation.Win2008R2Checked) {
                SB.AppendLine("	<li>Windows Server 2008 R2 all editions</li>");
            } else {
                if (cmdlet.SupportInformation.Win2008R2StdChecked) {
                    SB.AppendLine("	<li>Windows Server 2008 R2 Standard</li>");
                }
                if (cmdlet.SupportInformation.Win2008R2EEChecked) {
                    SB.AppendLine("	<li>Windows Server 2008 R2 Enterprise</li>");
                }
                if (cmdlet.SupportInformation.Win2008R2DCChecked) {
                    SB.AppendLine("	<li>Windows Server 2008 R2 Datacenter</li>");
                }
            }
            if (cmdlet.SupportInformation.Win2012Checked) {
                SB.AppendLine("	<li>Windows Server 2012 all editions</li>");
            } else {
                if (cmdlet.SupportInformation.Win2012StdChecked) {
                    SB.AppendLine("	<li>Windows Server 2012 Standard</li>");
                }
                if (cmdlet.SupportInformation.Win2012DCChecked) {
                    SB.AppendLine("	<li>Windows Server 2012 Datacenter</li>");
                }
            }
            if (cmdlet.SupportInformation.Win2012R2Checked) {
                SB.AppendLine("	<li>Windows Server 2012 R2 all editions</li>" );
            } else {
                if (cmdlet.SupportInformation.Win2012R2StdChecked) {
                    SB.AppendLine("	<li>Windows Server 2012 R2 Standard</li>");
                }
                if (cmdlet.SupportInformation.Win2012R2DCChecked) {
                    SB.AppendLine("	<li>Windows Server 2012 R2 Datacenter</li>");
                }
            }
            if (cmdlet.SupportInformation.Win2016Checked) {
                SB.AppendLine("	<li>Windows Server 2016 all editions</li>");
            } else {
                if (cmdlet.SupportInformation.Win2016StdChecked) {
                    SB.AppendLine("	<li>Windows Server 2016 Standard</li>");
                }
                if (cmdlet.SupportInformation.Win2016DCChecked) {
                    SB.AppendLine("	<li>Windows Server 2016 Datacenter</li>");
                }
            }
            if (cmdlet.SupportInformation.Win2019Checked) {
                SB.AppendLine("	<li>Windows Server 2019 all editions</li>");
            } else {
                if (cmdlet.SupportInformation.Win2019StdChecked) {
                    SB.AppendLine("	<li>Windows Server 2019 Standard</li>");
                }
                if (cmdlet.SupportInformation.Win2019DCChecked) {
                    SB.AppendLine("	<li>Windows Server 2019 Datacenter</li>");
                }
            }
            SB.Append("</ul>");
        }

        // generates HTML for web browser control
        public static Task<String> GenerateHtmlView(CmdletObject cmdlet, ModuleObject moduleObject) {
            return Task<String>.Factory.StartNew(() => generatePureHtml(cmdlet, moduleObject.Cmdlets, new StringBuilder(), moduleObject.UseSupports));
        }
    }
}
