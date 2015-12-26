///////////////////////////////////////////////////////////////////////////////////////////////////////////
// The code below is an adopted code from a Codekicker.BBCode project.
// Project home page https://bbcode.codeplex.com/
// Original source version: 5.0
// 
// Author's website: http://codekicker.de
// Licensed under the Creative Commons Attribution 3.0 Licence: http://creativecommons.org/licenses/by/3.0/
///////////////////////////////////////////////////////////////////////////////////////////////////////////

using CodeKicker.BBCode.SyntaxTree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CodeKicker.BBCode {
	public static class BBCode {
		static readonly BBCodeParser defaultParser = GetParser();
		static BBCodeParser GetParser() {
			return new BBCodeParser(ErrorMode.ErrorFree, null, new[]
                {
                    new BBTag("b", "<strong>", "</strong>"), 
                    new BBTag("i", "<span style=\"font-style:italic;\">", "</span>"), 
                    new BBTag("u", "<span style=\"text-decoration:underline;\">", "</span>"), 
                    new BBTag("code", "<pre class=\"prettyprint\">", "</pre>"), 
                    new BBTag("img", "<img src=\"${content}\" />", "", false, true), 
                    new BBTag("quote", "<blockquote>", "</blockquote>"), 
                    new BBTag("list", "<ul>", "</ul>"), 
                    new BBTag("*", "<li>", "</li>", true, false), 
                    new BBTag("url", "<a href=\"${href}\">", "</a>", new BBAttribute("href", ""), new BBAttribute("href", "href")), 
                });
		}

		public static readonly String InvalidBBCodeTextChars = @"[]\";

		/// <summary>
		/// Transforms the given BBCode into safe HTML with the default configuration from http://codekicker.de
		/// This method is thread safe.
		/// In order to use this library, we require a link to http://codekicker.de/ from you. Licensed unter the Creative Commons Attribution 3.0 Licence: http://creativecommons.org/licenses/by/3.0/.
		/// </summary>
		/// <param name="bbCode">A non-null string of valid BBCode.</param>
		/// <returns></returns>
		public static String ToHtml(String bbCode) {
			if (bbCode == null) { throw new ArgumentNullException("bbCode"); }
			return defaultParser.ToHtml(bbCode);
		}
		/// <summary>
		/// Transforms the given BBCode into safe HTML with the default configuration from http://codekicker.de
		/// This method is thread safe.
		/// In order to use this library, we require a link to http://codekicker.de/ from you. Licensed unter the Creative Commons Attribution 3.0 Licence: http://creativecommons.org/licenses/by/3.0/.
		/// </summary>
		/// <param name="bbCode">A non-null string of valid BBCode.</param>
		/// <returns></returns>
		public static String ToHtml(BBCodeParser rules, String bbCode) {
			if (rules == null) {
				rules = defaultParser;
			}
			if (bbCode == null) { throw new ArgumentNullException("bbCode"); }
			return rules.ToHtml(bbCode);
		}
		/// <summary>
		/// Encodes an arbitrary String to be valid BBCode. Example: "[b]" => "\[b\]". The resulting String is safe against
		/// BBCode-Injection attacks.
		/// In order to use this library, we require a link to http://codekicker.de/ from you. Licensed unter the Creative Commons Attribution 3.0 Licence: http://creativecommons.org/licenses/by/3.0/.
		/// </summary>
		public static String EscapeText(String text) {
			if (text == null) throw new ArgumentNullException("text");

			Int32 escapeCount = text.Count(t => t == '[' || t == ']' || t == '\\');
			if (escapeCount == 0) { return text; }
			Char[] output = new Char[text.Length + escapeCount];
			Int32 outputWritePos = 0;
			foreach (Char c in text) {
				if (c == '[' || c == ']' || c == '\\') {
					output[outputWritePos++] = '\\';
				}
				output[outputWritePos++] = c;
			}

			return new String(output);
		}
		/// <summary>
		/// Decodes a String of BBCode that only contains text (no tags). Example: "\[b\]" => "[b]". This is the reverse
		/// oepration of EscapeText.
		/// In order to use this library, we require a link to http://codekicker.de/ from you. Licensed unter the Creative Commons Attribution 3.0 Licence: http://creativecommons.org/licenses/by/3.0/.
		/// </summary>
		public static String UnescapeText(String text) {
			if (text == null) { throw new ArgumentNullException("text"); }
			return text.Replace("\\[", "[").Replace("\\]", "]").Replace("\\\\", "\\");
		}
		public static SyntaxTreeNode ReplaceTextSpans(SyntaxTreeNode node, Func<String, IList<TextSpanReplaceInfo>> getTextSpansToReplace, Func<TagNode, bool> tagFilter) {
			if (node == null) throw new ArgumentNullException("node");
			if (getTextSpansToReplace == null) throw new ArgumentNullException("getTextSpansToReplace");

			TextNode Node = node as TextNode;
			if (Node != null) {
				String text = Node.Text;

				IList<TextSpanReplaceInfo> replacements = getTextSpansToReplace(text);
				if (replacements == null || replacements.Count == 0) {
					return node;
				}

				List<SyntaxTreeNode> replacementNodes = new List<SyntaxTreeNode>(replacements.Count * 2 + 1);
				Int32 lastPos = 0;

				foreach (TextSpanReplaceInfo r in replacements) {
					if (r.Index < lastPos) {
						throw new ArgumentException("the replacement text spans must be ordered by index and non-overlapping");
					}
					if (r.Index > text.Length - r.Length) {
						throw new ArgumentException("every replacement text span must reference a range within the text node");
					}

					if (r.Index != lastPos) {
						replacementNodes.Add(new TextNode(text.Substring(lastPos, r.Index - lastPos)));
					}

					if (r.Replacement != null) {
						replacementNodes.Add(r.Replacement);
					}

					lastPos = r.Index + r.Length;
				}

				if (lastPos != text.Length) {
					replacementNodes.Add(new TextNode(text.Substring(lastPos)));
				}

				return new SequenceNode(replacementNodes);
			}
			List<SyntaxTreeNode> fixedSubNodes = node.SubNodes.Select(n => {
				if (n is TagNode && (tagFilter != null && !tagFilter((TagNode)n))) return n; //skip filtered tags

				SyntaxTreeNode repl = ReplaceTextSpans(n, getTextSpansToReplace, tagFilter);
				Debug.Assert(repl != null);
				return repl;
			}).ToList();

			return fixedSubNodes.SequenceEqual(node.SubNodes, ReferenceEqualityComparer<SyntaxTreeNode>.Instance)
				? node
				: node.SetSubNodes(fixedSubNodes);
		}
		public static void VisitTextNodes(SyntaxTreeNode node, Action<String> visitText, Func<TagNode, bool> tagFilter) {
			if (node == null) {
				throw new ArgumentNullException("node");
			}
			if (visitText == null) {
				throw new ArgumentNullException("visitText");
			}

			TextNode Node = node as TextNode;
			if (Node != null) {
				visitText(Node.Text);
			} else {
				if (node is TagNode && (tagFilter != null && !tagFilter((TagNode)node))) {
					return;
				} //skip filtered tags

				foreach (var subNode in node.SubNodes) {
					VisitTextNodes(subNode, visitText, tagFilter);
				}
			}
		}

		class ReferenceEqualityComparer<T> : IEqualityComparer<T>
			where T : class {
			public static readonly ReferenceEqualityComparer<T> Instance = new ReferenceEqualityComparer<T>();

			public bool Equals(T x, T y) {
				return ReferenceEquals(x, y);
			}

			public Int32 GetHashCode(T obj) {
				return obj == null ? 0 : obj.GetHashCode();
			}
		}
	}
}
