using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using PsCmdletHelpEditor.Core.Models.Xml;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

class PsCommandParameterCollection : PsCommandParameterCollectionBase<PsCommandParameter> {
    readonly Dictionary<String, PsCommandParameter> _dictionary = new(StringComparer.OrdinalIgnoreCase);

    public void ImportCommentBasedHelp(PSObject cbh) {
        if (cbh.Members["parameters"] is not null) {
            List<PSObject> cbhParams = [];
            PSObject parameters = (PSObject)cbh.Members["parameters"].Value;
            if (parameters.Members["parameter"]?.Value is PSObject singleValue) {
                cbhParams.Add(singleValue);
            } else if (parameters.Members["parameter"]?.Value is PSObject[] multiValue) {
                cbhParams.AddRange(multiValue);
            }

            foreach (PSObject cbhParam in cbhParams) {
                String name = (String)((PSObject)cbhParam.Members["name"].Value).BaseObject;
                if (_dictionary.TryGetValue(name, out PsCommandParameter param)) {
                    param.ImportCommentBasedHelp(cbhParam);
                }
            }
        }
    }
    public void ImportMamlHelp(MamlXmlNode commandNode) {
        MamlXmlNodeList? nodes = commandNode.SelectNodes("command:parameters/command:parameter");
        if (nodes is null) {
            return;
        }

        foreach (MamlXmlNode node in nodes) {
            MamlXmlNode? tempNode = node.SelectSingleNode("maml:name");
            if (tempNode is null) {
                continue;
            }
            String name = tempNode.InnerText;
            PsCommandParameter? param = InternalList.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (param is null) {
                param = PsCommandParameter.FromMaml(name, node);
                InternalList.Add(param);
            } else {
                param.ImportMamlHelp(commandNode);
            }
        }
    }
    public void FromCmdlet(PSObject cmdlet) {
        if (cmdlet.Members["ParameterSets"].Value != null) {
            var paramSets = new List<CommandParameterSetInfo>((IEnumerable<CommandParameterSetInfo>)cmdlet.Members["ParameterSets"].Value);
            foreach (CommandParameterSetInfo paramSet in paramSets) {
                if (paramSet.Parameters.Count == 0) { return; }

                foreach (CommandParameterInfo param in paramSet.Parameters.Where(param => !ExcludedParameters.Contains(param.Name, StringComparer.OrdinalIgnoreCase))) {
                    var psParam = PsCommandParameter.FromCmdlet(param);
                    if (!InternalList.Contains(psParam)) {
                        InternalList.Add(psParam);
                        _dictionary[psParam.Name] = psParam;
                    }
                }
            }
        }
    }
}