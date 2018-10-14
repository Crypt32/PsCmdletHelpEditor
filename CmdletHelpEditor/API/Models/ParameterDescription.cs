using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management.Automation;
using System.Xml.Serialization;

namespace CmdletHelpEditor.API.Models {
    public class ParameterDescription : INotifyPropertyChanged {
        String description, defaultValue;
        Boolean globbing;
        ItemStatus status = ItemStatus.New;

        public ParameterDescription() {
            Status = ItemStatus.Missing;
            Attributes = new List<String>();
            Aliases = new List<String>();
        }
        public ParameterDescription(CommandParameterInfo param) {
            m_initialize(param);
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
        public List<String> Attributes { get; set; }
        public List<String> Aliases { get; set; }
        public String Description {
            get => description ?? String.Empty;
            set {
                if (description != value) {
                    description = value;
                    status = ItemStatus.Incomplete;
                    OnPropertyChanged(nameof(Description));
                    OnPropertyChanged(nameof(Status));
                }
            }
        }
        public String DefaultValue {
            get => defaultValue ?? String.Empty;
            set {
                if (defaultValue != value) {
                    defaultValue = value;
                    OnPropertyChanged(nameof(DefaultValue));
                }
            }
        }
        [XmlAttribute("globbing")]
        public Boolean Globbing {
            get => globbing;
            set {
                globbing = value;
                OnPropertyChanged(nameof(Globbing));
            }
        }
        [XmlIgnore]
        public ItemStatus Status {
            get {
                if (status == ItemStatus.Missing || status == ItemStatus.New) { return status; }
                return String.IsNullOrEmpty(Description)
                    ? ItemStatus.Incomplete
                    : ItemStatus.Valid;
            }
            set => status = value;
        }

        void m_initialize(CommandParameterInfo param) {
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
            Attributes = new List<String>();
            if (param.Attributes.Count > 0) {
                foreach (Attribute item in param.Attributes) {
                    Attributes.Add(item.ToString());
                }
            }
            // process parameter aliases
            Aliases = new List<String>();
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
        Boolean Equals(ParameterDescription other) {
            return String.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase) &&
                String.Equals(Type, other.Type, StringComparison.InvariantCultureIgnoreCase);
        }
        public override Int32 GetHashCode() {
            unchecked {
                return (Name.GetHashCode() * 397) ^ Type.GetHashCode();
            }
        }
        public override Boolean Equals(Object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            return obj is ParameterDescription other && Equals(other);
        }

        void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
