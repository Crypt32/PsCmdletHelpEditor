using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

class PsCommandInfo : IPsCommandInfo {
    readonly PsCommandParameterSetCollection _paramSets = new();
    readonly PsCommandParameterCollection _params = new();
    readonly PsCommandExampleCollection _examples = new();
    readonly PsCommandRelatedLinkCollection _relatedLinks = new();
    readonly List<String> _syntax = [];
    PsCommandGeneralDescription? generalDescription;

    PsCommandInfo() { }

    public String Name { get; private set; } = null!;
    public String? Verb { get; private set; }
    public String? Noun { get; private set; }
    public String? ExtraHeader { get; }
    public String? ExtraFooter { get; }
    public IPsCommandGeneralDescription GetDescription() {
        return generalDescription;
    }
    public IReadOnlyList<String> GetSyntax() {
        return _syntax;
    }
    public IReadOnlyList<IPsCommandParameterSetInfo> GetParameterSets() {
        return _paramSets;
    }
    public IReadOnlyList<IPsCommandParameterDescription> GetParameters() {
        return _params;
    }
    public IReadOnlyList<IPsCommandExample> GetExamples() {
        return _examples;
    }
    public IReadOnlyList<IPsCommandRelatedLink> GetRelatedLinks() {
        return _relatedLinks;
    }
    public IPsCommandSupportInfo? GetSupportInfo() {
        throw new NotImplementedException();
    }

    public void ImportCommentBasedHelp(PSObject cbh) {
        generalDescription!.ImportCommentBasedHelp(cbh);
        _examples.ImportCommentBasedHelp(cbh);
        _relatedLinks.ImportCommentBasedHelp(cbh);
    }

    /// <summary>
    /// Creates an instance of <see cref="PsCommandInfo"/> from native PowerShell command.
    /// </summary>
    /// <param name="cmdlet">Command info from "Get-Command" output.</param>
    /// <param name="includeCommentBasedHelp">Specifies whether to try to fetch comment-based help.</param>
    /// <returns>PS command definition.</returns>
    public static PsCommandInfo FromCommandInfo(PSObject cmdlet, Boolean includeCommentBasedHelp = false) {
        var retValue = new PsCommandInfo {
            Name = (String)cmdlet.Members["Name"].Value,
            Verb = cmdlet.Members["Verb"]?.ToString(),
            Noun = cmdlet.Members["Noun"]?.ToString(),
            generalDescription = PsCommandGeneralDescription.FromCmdlet(cmdlet)
        };
        retValue._paramSets.FromCmdlet(cmdlet);
        retValue._params.FromCmdlet(cmdlet);
        // prepend syntax with command name as syntax generator doesn't have access to command name.
        // Though leading space is provided.
        retValue._syntax.AddRange(retValue._paramSets.GetParamSetSyntax().Select(x => retValue.Name + x));

        // last condition is just sanity check if command includes comment-based help and if it is worth to move on.
        if (includeCommentBasedHelp
            // this 'syn' is custom member added by editor.
            && cmdlet.Members["syn"].Value is PSObject cbh
            && cbh.Members["Synopsis"] is not null
            && cbh.Members["Description"] is not null) {
            retValue.ImportCommentBasedHelp(cbh);
        }

        return retValue;
    }
}