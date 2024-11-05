using System;
using PsCmdletHelpEditor.XmlRpc;

namespace PsCmdletHelpEditor.Core.Models.Xml;

public class XmlRpcProviderInformation : IXmlRpcProviderInformation {
    public String ProviderName { get; set; }
    public String ProviderURL { get; set; }
    public XmlRpcBlogInfo Blog { get; set; }
    public String UserName { get; set; }
    public String Password { get; set; }
    public Int32 FetchPostCount { get; set; }
}