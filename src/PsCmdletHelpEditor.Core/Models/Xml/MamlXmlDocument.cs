using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace PsCmdletHelpEditor.Core.Models.Xml;

/// <summary>
/// Represents a MAML-specific wrapper for <see cref="XmlDocument"/> class.
/// </summary>
class MamlXmlDocument {
    readonly XmlDocument _doc = new();

    XmlNamespaceManager? ns;

    void loadNamespaces() {
        ns = new XmlNamespaceManager(_doc.NameTable);
        ns.AddNamespace("command", "http://schemas.microsoft.com/maml/dev/command/2004/10");
        ns.AddNamespace("maml", "http://schemas.microsoft.com/maml/2004/10");
        ns.AddNamespace("dev", "http://schemas.microsoft.com/maml/dev/2004/10");
    }

    /// <inheritdoc cref="XmlDocument.Load(String)"/>
    public void Load(String fileName) {
        _doc.Load(fileName);
        loadNamespaces();
    }
    /// <inheritdoc cref="XmlDocument.LoadXml(String)"/>
    public void LoadXml(String xml) {
        _doc.LoadXml(xml);
        loadNamespaces();
    }
    /// <inheritdoc cref="XmlNode.SelectSingleNode(String)"/>
    public MamlXmlNode? SelectSingleNode(String xPath) {
        XmlNode? node = _doc.SelectSingleNode(xPath, ns);
        if (node is null) {
            return null;
        }

        return new MamlXmlNode(node, ns);
    }/// <inheritdoc cref="XmlNode.SelectNodes(String)"/>
    public MamlXmlNodeList? SelectNodes(String xPath) {
        var nodes = _doc.SelectNodes(xPath, ns);
        if (nodes is null) {
            return null;
        }

        return new MamlXmlNodeList(nodes, ns);
    }
}
/// <summary>
/// Represents a wrapper around <see cref="XmlNode"/> class.
/// </summary>
/// <param name="Node"></param>
/// <param name="Ns"></param>
class MamlXmlNode(XmlNode Node, XmlNamespaceManager? Ns) : IEnumerable {
    /// <inheritdoc cref="XmlNode.SelectSingleNode(String)"/>
    public MamlXmlNode? SelectSingleNode(String xPath) {
        var node = Node.SelectSingleNode(xPath, Ns);
        if (node is null) {
            return null;
        }
        return new MamlXmlNode(node, Ns);
    }

    /// <inheritdoc cref="XmlNode.InnerText"/>
    public String InnerText => Node.InnerText.Trim();
    /// <inheritdoc cref="XmlNode.ChildNodes"/>
    public MamlXmlNodeList ChildNodes {
        get {
            if (Node.HasChildNodes) {
                return new MamlXmlNodeList(Node.ChildNodes, Ns);
            }

            return null;
        }
    }

    /// <inheritdoc cref="XmlNode.SelectNodes(String)"/>
    public MamlXmlNodeList? SelectNodes(String xPath) {
        XmlNodeList? nodes = Node.SelectNodes(xPath, Ns);
        if (nodes is null) {
            return null;
        }

        return new MamlXmlNodeList(nodes, Ns);
    }
    /// <inheritdoc cref="XmlNode.Attributes"/>
    public XmlAttributeCollection? Attributes => Node.Attributes;
    public IEnumerator GetEnumerator() {
        return Node.GetEnumerator();
    }
}

/// <summary>
/// Represents a wrapper around <see cref="XmlNodeList"/> class.
/// </summary>
class MamlXmlNodeList : IEnumerable<MamlXmlNode> {
    readonly List<MamlXmlNode> _nodeList = [];
    readonly XmlNodeList _xmlNodeList;
    readonly StringBuilder _sb = new();

    public MamlXmlNodeList(XmlNodeList xmlNodeList, XmlNamespaceManager? ns) {
        _xmlNodeList = xmlNodeList;
        foreach (XmlNode node in xmlNodeList) {
            _nodeList.Add(new MamlXmlNode(node, ns));
        }
    }

    /// <inheritdoc cref="XmlNodeList.Count"/>
    public Int32 Count => _nodeList.Count;
    /// <inheritdoc cref="XmlNodeList.this[Int32]"/>
    public MamlXmlNode this[Int32 index] => _nodeList[index];

    public String ReadMamlParagraphs() {
        if (_xmlNodeList.Count == 0) {
            return String.Empty;
        }
        _sb.Clear();
        foreach (XmlNode node in _xmlNodeList) {
            _sb.AppendLine(Regex.Replace(node.InnerText, "(?<!\r)\n", "\r\n"));
            _sb.AppendLine();
        }


        return _sb.ToString().TrimEnd();
    }
    IEnumerator<MamlXmlNode> IEnumerable<MamlXmlNode>.GetEnumerator() {
        return _nodeList.GetEnumerator();
    }
    /// <inheritdoc />
    public IEnumerator GetEnumerator() {
        return _nodeList.GetEnumerator();
    }
}