using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using PsCmdletHelpEditor.Core.Models.Xml;

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

    void setTypes(IEnumerable<String> types, IEnumerable<String> urls, IEnumerable<String> descriptions, Boolean input) {
        if (input) {
            InputType = String.Join(";", types);
            InputUrl = String.Join(";", urls);
            InputTypeDescription = String.Join(";", descriptions);
        } else {
            ReturnType = String.Join(";", types);
            ReturnUrl = String.Join(";", urls);
            ReturnTypeDescription = String.Join(";", descriptions);
        }
    }
         
    #region Import from PS Object/Comment-based help

    void importCbhSynopsisFromPsObject(PSObject cbh) {
        Synopsis = (String)cbh.Members["Synopsis"].Value;
    }
    void importCbhDescriptionFromPsObject(PSObject cbh) {
        Description = ((PSObject[])cbh.Members["Description"].Value)
            .Aggregate(String.Empty, (current, paragraph) => current + paragraph.Members["Text"].Value + Environment.NewLine)
            .TrimEnd();
    }
    void importCbhNotesFromPsObject(PSObject cbh) {
        if (cbh.Members["alertSet"].Value is PSObject note) {
            Notes = (String)((PSObject[])note.Members["alert"].Value)[0].Members["Text"].Value;
        }
    }
    void importTypesFromPsObject(PSObject cbh, Boolean input) {
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

        var types = new List<String>();
        var urls = new List<String>();
        var descriptions = new List<String>();

        foreach (PSObject type in iType) {
            var internalType = (PSObject)type.Members["type"].Value;
            if (internalType.Properties["name"] is null) {
                types.Add(String.Empty);
            } else {
                var name = (PSObject)internalType.Members["name"].Value;
                types.Add((String)name.BaseObject);
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
        setTypes(types, urls, descriptions, input);
    }

    #endregion

    #region Import from MAML help

    void importSynopsisFromMaml(MamlXmlNode commandNode) {
        MamlXmlNode? tempNode = commandNode.SelectSingleNode("command:details/maml:description");
        if (tempNode is not null) {
            Synopsis = tempNode.ChildNodes.ReadMamlParagraphs();
        }
    }
    void importDescriptionFromMaml(MamlXmlNode commandNode) {
        MamlXmlNode? tempNode = commandNode.SelectSingleNode("maml:description");
        if (tempNode is not null) {
            Description = tempNode.ChildNodes.ReadMamlParagraphs();
        }
    }
    void importNotesFromMaml(MamlXmlNode commandNode) {
        MamlXmlNode? tempNode = commandNode.SelectSingleNode("maml:alertSet/maml:alert");
        if (tempNode is not null) {
            Description = tempNode.ChildNodes.ReadMamlParagraphs();
        }
    }
    void importTypesFromMaml(MamlXmlNode commandNode, Boolean input) {
        String topNodeName = input ? "inputType" : "returnValue";

        MamlXmlNodeList? nodes = commandNode.SelectNodes($"command:inputTypes/command:{topNodeName}");
        if (nodes is null) {
            return;
        }

        List<String> types = [];
        List<String> urls = [];
        List<String> descriptions = [];
        foreach (MamlXmlNode typeNode in nodes) {
            MamlXmlNode? tempNode = typeNode.SelectSingleNode("dev:type/maml:name");
            if (tempNode == null) { continue; }
            types.Add(tempNode.InnerText);
            tempNode = typeNode.SelectSingleNode("dev:type/maml:uri");
            if (tempNode != null) {
                urls.Add(tempNode.InnerText);
            }
            String descriptionNodes = String.Empty;
            tempNode = typeNode.SelectSingleNode("dev:type/maml:description");
            if (tempNode != null) {
                descriptionNodes += tempNode.ChildNodes.ReadMamlParagraphs();
            }
            tempNode = typeNode.SelectSingleNode("maml:description");
            if (tempNode != null) {
                descriptionNodes += tempNode.ChildNodes;
            }
            descriptions.Add(descriptionNodes);
        }
        setTypes(types, urls, descriptions, input);
    }

    #endregion

    public void ImportCommentBasedHelp(PSObject cbh) {
        importCbhSynopsisFromPsObject(cbh);
        importCbhDescriptionFromPsObject(cbh);
        importCbhNotesFromPsObject(cbh);
        importTypesFromPsObject(cbh, true);
        importTypesFromPsObject(cbh, false);
    }
    public void ImportFromMamlHelp(MamlXmlNode commandNode) {
        importSynopsisFromMaml(commandNode);
        importDescriptionFromMaml(commandNode);
        importNotesFromMaml(commandNode);
        importTypesFromMaml(commandNode, true);
        importTypesFromMaml(commandNode, false);
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
