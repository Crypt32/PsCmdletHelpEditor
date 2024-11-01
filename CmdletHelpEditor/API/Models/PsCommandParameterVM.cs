using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Xml.Serialization;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Models.Xml;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.Models;
public class PsCommandParameterVM : ViewModelBase {
    String description, defaultValue;
    Boolean globbing;
    ItemStatus status = ItemStatus.New;

    public PsCommandParameterVM() {
        Status = ItemStatus.Missing;
    }
    public PsCommandParameterVM(CommandParameterInfo param) : this() {
        initialize(param);
    }

    public String Name { get; set; }
    [XmlAttribute("type")]
    public String Type { get; set; }
    [XmlAttribute("varLen")]
    public Boolean AcceptsArray { get; set; }
    [XmlAttribute("required")]
    public Boolean Mandatory { get; set; }
    [XmlAttribute("dynamic")]
    public Boolean Dynamic { get; set; }
    [XmlAttribute("pipeRemaining")]
    public Boolean RemainingArgs { get; set; }
    [XmlAttribute("pipe")]
    public Boolean Pipeline { get; set; }
    [XmlAttribute("pipeProp")]
    public Boolean PipelinePropertyName { get; set; }
    [XmlAttribute("isPos")]
    public Boolean Positional { get; set; }
    [XmlAttribute("pos")]
    public String Position { get; set; }
    public List<String> Attributes { get; set; } = [];
    public List<String> Aliases { get; set; } = [];
    public String Description {
        get => description ?? String.Empty;
        set {
            if (description != value) {
                description = value;
                status = ItemStatus.Incomplete;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Status));
            }
        }
    }
    public String DefaultValue {
        get => defaultValue ?? String.Empty;
        set {
            if (defaultValue != value) {
                defaultValue = value;
                OnPropertyChanged();
            }
        }
    }
    [XmlAttribute("globbing")]
    public Boolean Globbing {
        get => globbing;
        set {
            globbing = value;
            OnPropertyChanged();
        }
    }
    [XmlIgnore]
    public ItemStatus Status {
        get {
            if (status is ItemStatus.Missing or ItemStatus.New) { return status; }
            return String.IsNullOrEmpty(Description)
                ? ItemStatus.Incomplete
                : ItemStatus.Valid;
        }
        set => status = value;
    }

    public XmlPsCommandParameterDescription ToXmlObject() {
        return new XmlPsCommandParameterDescription {
            Name = Name,
            Type = Type,
            AcceptsArray = AcceptsArray,
            Mandatory = Mandatory,
            Dynamic = Dynamic,
            RemainingArgs = RemainingArgs,
            Pipeline = Pipeline,
            PipelinePropertyName = PipelinePropertyName,
            Globbing = Globbing,
            Positional = Positional,
            Position = Position,
            Description = Description,
            DefaultValue = DefaultValue,
            Aliases = Aliases,
            Attributes = Attributes
        };
    }

    public static PsCommandParameterVM ImportFromCommandInfo(IPsCommandParameterDescription param) {
        var retValue = new PsCommandParameterVM {
            Name = param.Name,
            Type = param.Type,
            AcceptsArray = param.AcceptsArray,
            Mandatory = param.Mandatory,
            Globbing = param.Globbing,
            Dynamic = param.Dynamic,
            RemainingArgs = param.RemainingArgs,
            Pipeline = param.Pipeline,
            PipelinePropertyName = param.PipelinePropertyName,
            Positional = param.Positional,
            Position = param.Position,
            Description = param.Description
        };
        if (param.IsOrphaned) {
            retValue.Status = ItemStatus.Missing;
        }
        retValue.Attributes.AddRange(param.GetAttributes());
        retValue.Aliases.AddRange(param.GetAliases());

        return retValue;
    }

    void initialize(CommandParameterInfo param) {
        status = ItemStatus.New;
        Name = param.Name;
        // get type
        getType(param);
        // get parameter parameters
        Mandatory = param.IsMandatory;
        Dynamic = param.IsDynamic;
        RemainingArgs = param.ValueFromRemainingArguments;
        Pipeline = param.ValueFromPipeline;
        PipelinePropertyName = param.ValueFromPipelineByPropertyName;
        // process position
        if (param.Position >= 0) {
            Position = Convert.ToString(param.Position);
            Positional = true;
        } else {
            Position = "named";
            Positional = false;
        }
        // process attributes
        if (param.Attributes.Count > 0) {
            foreach (Attribute item in param.Attributes) {
                Attributes.Add(item.ToString());
            }
        }
        // process parameter aliases
        if (param.Aliases.Count > 0) {
            foreach (String alias in param.Aliases) {
                Aliases.Add(alias);
            }
        }

    }
    void getType(CommandParameterInfo param) {
        String underlyingType = param.ParameterType.ToString();
        String genericType = String.Empty;
        String[] tokens;
        if (underlyingType.Contains("[")) { AcceptsArray = true; }
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

    public String PipelineInput {
        get {
            if (Pipeline || PipelinePropertyName) {
                String value = "true";
                if (Pipeline && !PipelinePropertyName) {
                    value += " (ByValue)";
                } else if (Pipeline && PipelinePropertyName) {
                    value += " (ByValue, ByPropertyName)";
                } else {
                    value += " (ByPropertyName)";
                }
                return value;
            }
            return "false";
        }
    }

    #region Equals

    public override Boolean Equals(Object obj) {
        if (ReferenceEquals(null, obj)) { return false; }
        if (ReferenceEquals(this, obj)) { return true; }
        return obj is PsCommandParameterVM other && Equals(other);
    }
    Boolean Equals(PsCommandParameterVM other) {
        return String.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase) &&
               String.Equals(Type, other.Type, StringComparison.InvariantCultureIgnoreCase);
    }
    public override Int32 GetHashCode() {
        unchecked {
            return (Name.GetHashCode() * 397) ^ Type.GetHashCode();
        }
    }

    #endregion
}
