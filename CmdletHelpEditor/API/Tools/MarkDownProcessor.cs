using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdletHelpEditor.API.Tools {
	class MarkDownProcessor {
		Char[] escapeShars = new[] {
			'#', '{', '}', '[', ']', '<', '>', '*', '+', '-', '|', '\\', '`', '_',
			'.'
		};

		static String EscapeMarkdown(String str) {

			return str;
		}
	}
}
