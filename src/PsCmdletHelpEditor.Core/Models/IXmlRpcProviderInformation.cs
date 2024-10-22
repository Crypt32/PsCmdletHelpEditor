using System;

namespace PsCmdletHelpEditor.Core.Models;

public interface IXmlRpcProviderInformation {
    BlogInfo Blog { get; set; }
    Int32 FetchPostCount { get; set; }
    String Password { get; set; }
    String ProviderName { get; set; }
    String ProviderURL { get; set; }
    String UserName { get; set; }
}