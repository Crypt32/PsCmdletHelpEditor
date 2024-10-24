using System;
using System.Collections.Generic;
using System.Management.Automation;
using PsCmdletHelpEditor.Core.Models.PowerShellNative;

namespace PsCmdletHelpEditor.Core.Models;

class PsCommandInfo : IPsCommandInfo {
    readonly PsCommandParameterSetCollection _paramSets = new();
    readonly PsCommandParameterCollection _params = new();
    readonly PsCommandExampleCollection _examples = new();
    readonly PsCommandRelatedLinkCollection _relatedLinks = new();
    readonly List<String> _syntax = [];

    PsCommandGeneralDescription generalDescription = new();

    public String Name { get; private set; } = null!;
    public String? Verb { get; private set; }
    public String? Noun { get; private set; }
    public List<String> Syntax { get; } = [];
    public String? ExtraHeader { get; }
    public String? ExtraFooter { get; }
    public IPsCommandGeneralDescription GetDescription() {
        return generalDescription;
    }
    public IReadOnlyList<String> GetSyntax() { throw new NotImplementedException(); }
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
    public IPsCommandSupportInfo? GetSupportInfo() { throw new NotImplementedException(); }

    void generateSyntax() {

    }

    public void ImportCommentBasedHelp(PSObject cbh) {

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

        if (includeCommentBasedHelp) {
            retValue.ImportCommentBasedHelp(cmdlet);
        }

        return retValue;
    }
}