using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;
class PsCommandGeneralDescription : IPsCommandGeneralDescription{
    PsCommandGeneralDescription() { }

    public String? Synopsis { get; private set; }
    public String? Description { get; private set; }
    public String? Notes { get; private set; }
    public String? InputType { get; private set; }
    public String? InputUrl { get; private set; }
    public String? ReturnType { get; private set; }
    public String? ReturnUrl { get; private set; }
    public String? InputTypeDescription { get; private set; }
    public String? ReturnTypeDescription { get; private set; }

    void importCbhSynopsis(PSObject cbh) {
        Synopsis = (String)cbh.Members["Synopsis"].Value;
    }
    void importCbhDescription(PSObject cbh) {
        Description = ((PSObject[])cbh.Members["Description"].Value)
            .Aggregate(String.Empty, (current, paragraph) => current + paragraph.Members["Text"].Value + Environment.NewLine)
            .TrimEnd();
    }
    void importCbhNotes(PSObject cbh) {
        if (cbh.Members["alertSet"].Value is PSObject note) {
            Notes = (String)((PSObject[])note.Members["alert"].Value)[0].Members["Text"].Value;
        }
    }
    void importTypes(PSObject cbh, Boolean input) {
        var iType = new List<PSObject>();
        String memberName = input ? "inputType" : "returnValue";
        // e.g. inputTypes, returnValues
        String containerName = memberName + 's';
        if (cbh.Members[containerName] is null) {
            return;
        }
        if (cbh.Members[memberName].Value is PSObject value) {
            iType.Add(value);
        } else if (cbh.Members[memberName].Value is PSObject[] multiValue) {
            iType.AddRange(multiValue);
        }

        var links = new List<String>();
        var urls = new List<String>();
        var descriptions = new List<String>();

        foreach (PSObject type in iType) {
            var internalType = (PSObject)type.Members["type"].Value;
            if (internalType.Properties["name"] is null) {
                links.Add(String.Empty);
            } else {
                var name = (PSObject)internalType.Members["name"].Value;
                links.Add((String)name.BaseObject);
            }
            if (internalType.Properties["uri"] is null) {
                urls.Add(String.Empty);
            } else {
                var uri = (PSObject)internalType.Members["uri"].Value;
                urls.Add((String)uri.BaseObject);
            }

            if (type.Properties["description"] is null) {
                continue;
            }
            if (type.Members["description"].Value is PSObject[] descriptionBase) {
                String description = descriptionBase
                    .Aggregate(String.Empty, (current, paragraph) => current + paragraph.Members["Text"].Value + Environment.NewLine);
                descriptions.Add(String.IsNullOrEmpty(description)
                    ? String.Empty
                    : description.TrimEnd());
            }
        }
        if (input) {
            InputType = String.Join(";", links);
            InputUrl = String.Join(";", urls);
            InputTypeDescription = String.Join(";", descriptions);
        } else {
            ReturnType = String.Join(";", links);
            ReturnUrl = String.Join(";", urls);
            ReturnTypeDescription = String.Join(";", descriptions);
        }
    }

    public void ImportCommentBasedHelp(PSObject cbh) {
        importCbhSynopsis(cbh);
        importCbhDescription(cbh);
        importCbhNotes(cbh);
        importTypes(cbh, true);
        importTypes(cbh, false);
    }

    public static PsCommandGeneralDescription FromCmdlet(PSObject cmdlet) {
        var retValue = new PsCommandGeneralDescription();
        PSMemberInfo outputTypeMember = cmdlet.Members["OutputType"];
        if (outputTypeMember?.Value is IEnumerable<PSTypeName> outputTypeNames) {
           retValue.ReturnType = String.Join(";", outputTypeNames.Select(x => x.Name));
        }

        return retValue;
    }
}
