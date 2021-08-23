using System;

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
