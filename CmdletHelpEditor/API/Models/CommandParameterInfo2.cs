using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CmdletHelpEditor.API.Models {
	public class CommandParameterInfo2 {
		String typeString;
		public String Name { get; set; }
		[XmlIgnore]
		public Type ParameterType { get; set; }
		public String TypeString {
			get { return typeString; }
			set {
				typeString = value;
				ParameterType = Type.GetType(typeString);
			}
		}
		public Boolean IsMandatory { get; set; }
		public Boolean IsDynamic { get; set; }
		public Boolean Position { get; set; }
		public Boolean ValueFromPipeline { get; set; }
		public Boolean ValueFromPipelineByPropertyName { get; set; }
		public Boolean ValueFromRemainingArguments { get; set; }
		public String HelpMessage { get; set; }
		public List<String> Aliases { get; set; }
		public List<Attribute> Attributes { get; set; }
	}
}
