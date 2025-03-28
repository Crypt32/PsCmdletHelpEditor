using System;
using PsCmdletHelpEditor.XmlRpc;

namespace PsCmdletHelpEditor.Core.Models;

public interface IXmlRpcProviderInformation {
    XmlRpcBlogInfo Blog { get; set; }
    Int32 FetchPostCount { get; set; }
    String Password { get; set; }
    String ProviderName { get; set; }
    String ProviderURL { get; set; }
    String UserName { get; set; }
}