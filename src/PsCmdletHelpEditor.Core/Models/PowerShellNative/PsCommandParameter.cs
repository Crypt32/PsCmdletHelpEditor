using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

class PsCommandParameter : IPsCommandParameterDescription {
    readonly List<String> _attributes = [];
    readonly List<String> _aliases = [];

    PsCommandParameter() { }

    public String Name { get; private set; } = null!;
    public String Type { get; private set; } = String.Empty;
    public String? Description { get; private set; }
    public Boolean AcceptsArray { get; private set; }
    public Boolean Mandatory { get; private set; }
    public Boolean Dynamic { get; private set; }
    public Boolean RemainingArgs { get; private set; }
    public Boolean Pipeline { get; private set; }
    public Boolean PipelinePropertyName { get; private set; }
    public Boolean Globbing { get; private set; }
    public Boolean Positional { get; private set; }
    public String? DefaultValue { get; private set; }
    public String Position { get; private set; } = String.Empty;
    public Boolean IsOrphaned { get; private set; }
    public IReadOnlyList<String> GetAttributes() {
        return _attributes;
    }
    public IReadOnlyList<String> GetAliases() {
        return _aliases;
    }

    void setParameterType(CommandParameterInfo param) {
        String underlyingType = param.ParameterType.ToString();
        String genericType = String.Empty;
        String[] tokens;
        if (underlyingType.Contains("[")) {
            AcceptsArray = true;
        }
        if (underlyingType.Contains("[") && !underlyingType.Contains("[]")) {
            tokens = underlyingType.Split('[');
            underlyingType = tokens[0];
            genericType = tokens[1].Replace("]", null);
            tokens = genericType.Split('.');
            genericType = tokens[tokens.Length - 1];
        }
        tokens = underlyingType.Split('.');
        Type = tokens[tokens.Length - 1];
        if (!String.IsNullOrEmpty(genericType)) {
            Type += "[" + genericType + "]";
        }
    }

    public void ImportCommentBasedHelp(PSObject cbh) {
        Description = ((PSObject[])cbh.Members["Description"].Value)
            .Aggregate(String.Empty, (current, paragraph) => current + paragraph.Members["Text"].Value + Environment.NewLine)
            .TrimEnd();
        DefaultValue = (String)((PSObject)cbh.Members["defaultValue"].Value).BaseObject;
    }
    public static PsCommandParameter FromCmdlet(CommandParameterInfo parameter) {
        var retValue = new PsCommandParameter {
            Name = parameter.Name,
            Description = parameter.HelpMessage,
            Mandatory = parameter.IsMandatory,
            Dynamic = parameter.IsDynamic,
            Pipeline = parameter.ValueFromPipeline,
            PipelinePropertyName = parameter.ValueFromPipelineByPropertyName,
            RemainingArgs = parameter.ValueFromRemainingArguments,
            Positional = parameter.Position >= 0,
            Position = parameter.Position >= 0 ? parameter.Position.ToString() : "named"
        };
        retValue.setParameterType(parameter);
        
        // process attributes
        if (parameter.Attributes.Count > 0) {
            foreach (Attribute attribute in parameter.Attributes) {
                retValue._attributes.Add(attribute.ToString());
            }
        }
        // process parameter aliases
        if (parameter.Aliases.Count > 0) {
            foreach (String alias in parameter.Aliases) {
                retValue._aliases.Add(alias);
            }
        }

        return retValue;
    }

    #region Equals

    public override Boolean Equals(Object? obj) {
        return obj is not null &&
               (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((PsCommandParameter)obj));
    }
    protected Boolean Equals(PsCommandParameter other) {
        return String.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }
    public override Int32 GetHashCode() {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
    }

    #endregion
}