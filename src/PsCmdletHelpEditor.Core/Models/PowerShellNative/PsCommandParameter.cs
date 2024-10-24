using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;

class PsCommandParameter : IPsCommandParameterDescription {
    readonly List<String> _attributes = [];
    readonly List<String> _aliases = [];

    PsCommandParameter() { }

    public String Name { get; private set; } = null!;
    public String Type { get; private set; } = String.Empty;
    public String? Description { get; private set; }
    public String? DefaultValue { get; private set; }
    public String Position { get; private set; } = String.Empty;
    public PsCommandParameterOption Options { get; private set; }
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
            Options |= PsCommandParameterOption.AcceptsArray;
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
    void setOptions(CommandParameterInfo parameter) {
        if (parameter.IsMandatory) {
            Options |= PsCommandParameterOption.Mandatory;
        }
        if (parameter.IsDynamic) {
            Options |= PsCommandParameterOption.Dynamic;
        }
        if (parameter.ValueFromPipeline) {
            Options |= PsCommandParameterOption.Pipeline;
        }
        if (parameter.ValueFromPipelineByPropertyName) {
            Options |= PsCommandParameterOption.PipelinePropertyName;
        }
        if (parameter.ValueFromRemainingArguments) {
            Options |= PsCommandParameterOption.RemainingArgs;
        }
        if (parameter.Position >= 0) {
            Options |= PsCommandParameterOption.Positional;
            Position = parameter.Position.ToString();
        } else {
            Position = "named";
        }
    }

    public static PsCommandParameter FromCmdlet(CommandParameterInfo parameter) {
        var retValue = new PsCommandParameter {
            Name = parameter.Name,
            Description = parameter.HelpMessage
        };
        
        retValue.setParameterType(parameter);
        retValue.setOptions(parameter);
        
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