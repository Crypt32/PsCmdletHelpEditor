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

namespace CodeKicker.BBCode.SyntaxTree
{
    public sealed class SequenceNode : SyntaxTreeNode
    {
        public SequenceNode()
        {
        }
        public SequenceNode(SyntaxTreeNodeCollection subNodes)
            : base(subNodes)
        {
            if (subNodes == null) throw new ArgumentNullException("subNodes");
        }
        public SequenceNode(IEnumerable<SyntaxTreeNode> subNodes)
            : base(subNodes)
        {
            if (subNodes == null) throw new ArgumentNullException("subNodes");
        }

        public override string ToHtml()
        {
            return string.Concat(SubNodes.Select(s => s.ToHtml()).ToArray());
        }
        public override string ToBBCode()
        {
            return string.Concat(SubNodes.Select(s => s.ToBBCode()).ToArray());
        }
        public override string ToText()
        {
            return string.Concat(SubNodes.Select(s => s.ToText()).ToArray());
        }

        public override SyntaxTreeNode SetSubNodes(IEnumerable<SyntaxTreeNode> subNodes)
        {
            if (subNodes == null) throw new ArgumentNullException("subNodes");
            return new SequenceNode(subNodes);
        }
        internal override SyntaxTreeNode AcceptVisitor(SyntaxTreeVisitor visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");
            return visitor.Visit(this);
        }
        protected override bool EqualsCore(SyntaxTreeNode b)
        {
            return true;
        }
    }
}
