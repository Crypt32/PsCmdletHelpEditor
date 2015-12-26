///////////////////////////////////////////////////////////////////////////////////////////////////////////
// The code below is an adopted code from a Codekicker.BBCode project.
// Project home page https://bbcode.codeplex.com/
// Original source version: 5.0
// 
// Author's website: http://codekicker.de
// Licensed under the Creative Commons Attribution 3.0 Licence: http://creativecommons.org/licenses/by/3.0/
///////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeKicker.BBCode.SyntaxTree
{
    public sealed class TextNode : SyntaxTreeNode
    {
        public TextNode(string text)
            : this(text, null)
        {
        }
        public TextNode(string text, string htmlTemplate)
            : base(null)
        {
            if (text == null) throw new ArgumentNullException("text");
            Text = text;
            HtmlTemplate = htmlTemplate;
        }

        public string Text { get; private set; }
        public string HtmlTemplate { get; private set; }

        public override string ToHtml()
        {
            return HtmlTemplate == null ? HttpUtility.HtmlEncode(Text) : HtmlTemplate.Replace("${content}", HttpUtility.HtmlEncode(Text));
        }
        public override string ToBBCode()
        {
            return Text.Replace("\\", "\\\\").Replace("[", "\\[").Replace("]", "\\]");
        }
        public override string ToText()
        {
            return Text;
        }

        public override SyntaxTreeNode SetSubNodes(IEnumerable<SyntaxTreeNode> subNodes)
        {
            if (subNodes == null) throw new ArgumentNullException("subNodes");
            if (subNodes.Any()) throw new ArgumentException("subNodes cannot contain any nodes for a TextNode");
            return this;
        }
        internal override SyntaxTreeNode AcceptVisitor(SyntaxTreeVisitor visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");
            return visitor.Visit(this);
        }

        protected override bool EqualsCore(SyntaxTreeNode b)
        {
            var casted = (TextNode)b;
            return Text == casted.Text && HtmlTemplate == casted.HtmlTemplate;
        }
    }
}