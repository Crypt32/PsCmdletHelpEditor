using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeKicker.BBCode;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Utils;

namespace PsCmdletHelpEditor.Core.Services.MAML;

public class PsMamlService : IMamlService {
    const String MAML_COMMAND_SYNTAX_TEMPLATE       = "MamlCommandSyntaxTemplate.hbs";
    const String MAML_COMMAND_DETAILS_TEMPLATE      = "MamlCommandDetailsTemplate.hbs";
    const String MAML_COMMAND_NOTES_TEMPLATE        = "MamlCommandNotesTemplate.hbs";
    const String MAML_COMMAND_INPUT_TYPES_TEMPLATE  = "MamlCommandInputTypesTemplate.hbs";
    const String MAML_COMMAND_RETURN_TYPES_TEMPLATE = "MamlCommandReturnTypes.hbs";
    const String MAML_COMMAND_ERRORS_TEMPLATE       = "MamlCommandErrorsTemplate.hbs";
    const String MAML_COMMAND_PARAM_TEMPLATE        = "MamlCommandParamTemplate.hbs";
    const String MAML_COMMAND_PARAM_VALUE_TEMPLATE  = "MamlCommandParamValueTemplate.hbs";
    const String MAML_COMMAND_EXAMPLE_TEMPLATE      = "MamlCommandExampleTeamplate.hbs";
    const String MAML_COMMAND_RELATED_LINK_TEMPLATE = "MamlCommandRelatedLinkTemplate.hbs";

    readonly BBCodeParser _bbRules = new(ErrorMode.ErrorFree, null, [
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
    
    #region Export MAML

    public async Task<String> XmlGenerateHelp(ICollection<IPsCommandInfo> cmdlets, IProgress? pb) {
    public async Task<String> ExportMamlHelp(ICollection<IPsCommandInfo> cmdlets, IProgress? pb) {
        if (cmdlets.Count == 0) {
            return String.Empty;
        }
        Double duration = 0;
        if (pb != null) {
            duration = 100.0 / cmdlets.Count;
        }
        var sb = new StringBuilder("<helpItems schema=\"maml\">");
        foreach (IPsCommandInfo cmdlet in cmdlets) {
            await xmlGenerateBodyAsync(_bbRules, sb, cmdlet);
            if (pb != null) {
                pb.Progress += duration;
            }
        }
        sb.Append("</helpItems>");
        return sb.ToString();
    }
    static Task xmlGenerateBodyAsync(BBCodeParser bbRules, StringBuilder sb, IPsCommandInfo cmdlet) {
        return Task.Factory.StartNew(() => xmlGenerateBody(bbRules, sb, cmdlet));
    }
    static void xmlGenerateBody(BBCodeParser bbRules, StringBuilder sb, IPsCommandInfo cmdlet) {
        sb.Append("<command:command xmlns:maml=\"http://schemas.microsoft.com/maml/2004/10\" xmlns:command=\"http://schemas.microsoft.com/maml/dev/command/2004/10\" xmlns:dev=\"http://schemas.microsoft.com/maml/dev/2004/10\" xmlns:MSHelp=\"http://msdn.microsoft.com/mshelp\">");
        sb.Append(xmlGenerateCmdletDetail(bbRules, cmdlet));
        sb.Append("<command:syntax>");
        IReadOnlyList<IPsCommandParameterDescription> parameters = cmdlet.GetParameters();
        // if current cmdlet hasn't parameters, then just write single syntaxItem
        if (parameters.Count == 0) {
            sb.AppendFormat(EmbeddedResourceReader.ReadFileAsString(MAML_COMMAND_SYNTAX_TEMPLATE),
                SecurityElement.Escape(cmdlet.Name));
        } else {
            xmlGenerateParameterSyntax(bbRules, sb, cmdlet);
        }
        sb.Append("</command:syntax><command:parameters>");
        if (parameters.Count > 0) {
            foreach (IPsCommandParameterDescription param in parameters) {
                sb.Append(xmlGenerateParameter(bbRules, param));
            }
        }
        sb.Append("</command:parameters>");
        sb.Append(xmlGenerateInputTypes(bbRules, cmdlet));
        sb.Append(xmlGenerateReturnTypes(bbRules, cmdlet));
        sb.Append(EmbeddedResourceReader.ReadFileAsString(MAML_COMMAND_ERRORS_TEMPLATE));
        sb.AppendFormat(EmbeddedResourceReader.ReadFileAsString(MAML_COMMAND_NOTES_TEMPLATE),
            generateParagraphs(cmdlet.GetDescription().Notes, bbRules));
        sb.Append("<command:examples>");
        foreach (IPsCommandExample item in cmdlet.GetExamples()) {
            sb.Append(xmlGenerateExamples(bbRules, item));
        }
        sb.Append("</command:examples><maml:relatedLinks>");
        foreach (IPsCommandRelatedLink link in cmdlet.GetRelatedLinks()) {
            sb.AppendFormat(EmbeddedResourceReader.ReadFileAsString(MAML_COMMAND_RELATED_LINK_TEMPLATE),
                SecurityElement.Escape(link.LinkText),
                SecurityElement.Escape(link.LinkUrl));
        }
        sb.Append("</maml:relatedLinks></command:command>");
    }
    static String xmlGenerateCmdletDetail(BBCodeParser bbRules, IPsCommandInfo cmdlet) {
        return String.Format(EmbeddedResourceReader.ReadFileAsString(MAML_COMMAND_DETAILS_TEMPLATE),
            SecurityElement.Escape(cmdlet.Name),
            generateParagraphs(cmdlet.GetDescription().Synopsis, bbRules),
            SecurityElement.Escape(cmdlet.Verb),
            SecurityElement.Escape(cmdlet.Noun),
            generateParagraphs(cmdlet.GetDescription().Description, bbRules));
    }
    static void xmlGenerateParameterSyntax(BBCodeParser bbRules, StringBuilder SB, IPsCommandInfo cmdlet) {
        String[] exclude = [
            "verbose","debug","erroraction","warningaction","errorvariable","warningvariable","outvariable","outbuffer","pipelinevariable"
        ];
        foreach (IPsCommandParameterSetInfo? paramSet in cmdlet.GetParameterSets()) {
            SB.Append("<command:syntaxItem>");
            SB.Append("<maml:name>" + SecurityElement.Escape(cmdlet.Name) + "</maml:name>");
            foreach (String paramSetParam in paramSet.GetParameters()) {
                if (exclude.Contains(paramSetParam.ToLower())) {
                    continue;
                }

                IPsCommandParameterDescription? param = cmdlet
                    .GetParameters()
                    .FirstOrDefault(x => String.Equals(x.Name, paramSetParam, StringComparison.CurrentCultureIgnoreCase));
                if (param == null) {
                    continue;
                }
                SB.Append("<command:parameter required=\"" + param.Mandatory.ToString().ToLower() + "\"");
                SB.Append(" variableLength=\"" + param.AcceptsArray.ToString().ToLower() + "\"");
                SB.Append(" globbing=\"" + param.Globbing.ToString().ToLower() + "\"");
                SB.Append(" pipelineInput=\"");
                if (param.Pipeline || param.PipelinePropertyName) {
                    SB.Append("true");
                    if (param is { Pipeline: true, PipelinePropertyName: false }) {
                        SB.Append(" (ByValue)");
                    } else if (param is { Pipeline: true, PipelinePropertyName: true }) {
                        SB.Append(" (ByValue, ByPropertyName)");
                    } else {
                        SB.Append(" (ByPropertyName)");
                    }
                } else {
                    SB.Append("false");
                }
                SB.Append("\" position=\"" + param.Position + "\">");
                SB.Append("<maml:name>" + SecurityElement.Escape(param.Name) + "</maml:name>");
                SB.Append("<maml:description>");
                SB.Append(generateParagraphs(param.Description, bbRules));
                SB.Append("</maml:description>");
                SB.Append("<command:parameterValue ");
                String paramValueRequired = "true";
                if (param.Type.ToLower() == "boolean" || param.Type.ToLower() == "switchparameter") {
                    paramValueRequired = "false";
                }
                //Additional parameter Values
                SB.Append("required=\"" + paramValueRequired + "\"");
                SB.Append(" variableLength=\"" + param.AcceptsArray.ToString().ToLower() + "\">");
                SB.Append(SecurityElement.Escape(param.Type));
                SB.Append("</command:parameterValue>");
                SB.Append("</command:parameter>");
            }
            SB.Append("</command:syntaxItem>");
        }
    }
    static String xmlGenerateParameter(BBCodeParser bbRules, IPsCommandParameterDescription param) {
        String paramValueRequired = "true";
        if (param.Type.ToLower() == "boolean" || param.Type.ToLower() == "switchparameter") {
            paramValueRequired = "false";
        }
        String paramValue = String.Format(EmbeddedResourceReader.ReadFileAsString(MAML_COMMAND_PARAM_VALUE_TEMPLATE),
            paramValueRequired,
            param.AcceptsArray.ToString().ToLower(),
            SecurityElement.Escape(param.Type));

        String pipelineInput = "false";
        if (param.Pipeline || param.PipelinePropertyName) {
            pipelineInput = "true";
            if (param is { Pipeline: true, PipelinePropertyName: false }) {
                pipelineInput += " (ByValue)";
            } else if (param is { Pipeline: true, PipelinePropertyName: true }) {
                pipelineInput += " (ByValue, ByPropertyName)";
            } else {
                pipelineInput += " (ByPropertyName)";
            }
        }

        return String.Format(EmbeddedResourceReader.ReadFileAsString(MAML_COMMAND_PARAM_TEMPLATE),
            param.Mandatory.ToString().ToLower(),
            param.AcceptsArray.ToString().ToLower(),
            param.Globbing.ToString().ToLower(),
            pipelineInput,
            param.Position,
            SecurityElement.Escape(param.Name),
            generateParagraphs(param.Description, bbRules),
            paramValue,
            SecurityElement.Escape(param.Type),
            SecurityElement.Escape(param.DefaultValue));
    }
    static String xmlGenerateInputTypes(BBCodeParser bbRules, IPsCommandInfo cmdlet) {
        var inputTypes = new List<String>(cmdlet.GetDescription().InputType?.Split(';') ?? []);
        var inputUrls = new List<String>(cmdlet.GetDescription().InputUrl?.Split(';') ?? []);
        var inputDescription = new List<String>(cmdlet.GetDescription().InputTypeDescription?.Split(';') ?? []);

        var SB = new StringBuilder("<command:inputTypes>");
        for (Int32 index = 0; index < inputTypes.Count; index++) {
            SB.AppendFormat(EmbeddedResourceReader.ReadFileAsString(MAML_COMMAND_INPUT_TYPES_TEMPLATE),
                bbRules.ToHtml(inputTypes[index], true),
                bbRules.ToHtml(inputUrls[index], true),
                generateParagraphs(inputDescription[index], bbRules));
        }
        SB.Append("	</command:inputTypes>");
        return SB.ToString();
    }
    static String xmlGenerateReturnTypes(BBCodeParser bbRules, IPsCommandInfo cmdlet) {
        var returnTypes = new List<String>(cmdlet.GetDescription().ReturnType?.Split(';') ?? []);
        var returnUrls = new List<String>(cmdlet.GetDescription().ReturnUrl?.Split(';') ?? []);
        var returnDescription = new List<String>(cmdlet.GetDescription().ReturnTypeDescription?.Split(';') ?? []);

        var SB = new StringBuilder("<command:returnValues>");
        for (Int32 index = 0; index < returnTypes.Count; index++) {
            SB.AppendFormat(EmbeddedResourceReader.ReadFileAsString(MAML_COMMAND_RETURN_TYPES_TEMPLATE),
                bbRules.ToHtml(returnTypes[index], true),
                bbRules.ToHtml(returnUrls[index], true),
                generateParagraphs(returnDescription[index], bbRules));
        }
        SB.Append("	</command:returnValues>");
        return SB.ToString();
    }
    static String xmlGenerateExamples(BBCodeParser bbRules, IPsCommandExample example) {
        return String.Format(EmbeddedResourceReader.ReadFileAsString(MAML_COMMAND_EXAMPLE_TEMPLATE),
            SecurityElement.Escape(example.Name),
            SecurityElement.Escape(example.Cmd),
            generateParagraphs(example.Description, bbRules),
            SecurityElement.Escape(example.Output));
    }
    static String generateParagraphs(String? input, BBCodeParser bbRules) {
        if (String.IsNullOrEmpty(input)) {
            return "<maml:para />";
        }

        input = Regex.Replace(input, "(?<!\r)\n", "\r\n");
        String[] temp = input.Split(["\r\n\r\n"], StringSplitOptions.RemoveEmptyEntries);
        for (Int32 index = 0; index < temp.Length; index++) {
            temp[index] = bbRules.ToHtml(temp[index], true);
        }
        return "<maml:para>" + String.Join("</maml:para><maml:para>", temp) + "</maml:para>";
    }

    #endregion
}