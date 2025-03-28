using System;
using CodeKicker.BBCode.SyntaxTree;

namespace CodeKicker.BBCode {
	public class TextSpanReplaceInfo {
		public TextSpanReplaceInfo(Int32 index, Int32 length, SyntaxTreeNode replacement) {
			if (index < 0) { throw new ArgumentOutOfRangeException("index"); }
			if (length < 0) { throw new ArgumentOutOfRangeException("index"); }

			Index = index;
			Length = length;
			Replacement = replacement;
		}

		public Int32 Index { get; private set; }
		public Int32 Length { get; private set; }
		public SyntaxTreeNode Replacement { get; private set; }
	}
}