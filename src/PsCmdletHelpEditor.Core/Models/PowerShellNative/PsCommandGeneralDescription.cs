using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;
class PsCommandGeneralDescription : IPsCommandGeneralDescription{
    public String? Synopsis { get; private set; }
    public String? Description { get; private set; }
    public String? Notes { get; private set; }
    public String? InputType { get; private set; }
    public String? InputUrl { get; private set; }
    public String? ReturnType { get; private set; }
    public String? ReturnUrl { get; private set; }
    public String? InputTypeDescription { get; private set; }
    public String? ReturnTypeDescription { get; private set; }

    public static PsCommandGeneralDescription FromCmdlet(PSObject cmdlet) {
        var retValue = new PsCommandGeneralDescription();
        PSMemberInfo outputTypeMember = cmdlet.Members["OutputType"];
        if (outputTypeMember?.Value is IEnumerable<PSTypeName> outputTypeNames) {
           retValue.ReturnType = String.Join(";", outputTypeNames.Select(x => x.Name));
        }

        return retValue;
    }
}
