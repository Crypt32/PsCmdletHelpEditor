using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CmdletHelpEditor.API.Models {
	public class CommandParameterSetInfo2 {
		public CommandParameterSetInfo2() {
			Parameters = new List<String>();
		}
		[XmlAttribute]
		public String Name { get; set; }
		[XmlAttribute("Params")]
		public List<String> Parameters { get; set; }
	}
}
