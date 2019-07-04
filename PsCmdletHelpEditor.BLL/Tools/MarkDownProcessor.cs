using System;

namespace PsCmdletHelpEditor.BLL.Tools {
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
