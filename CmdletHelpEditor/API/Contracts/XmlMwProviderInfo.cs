using System;

namespace CmdletHelpEditor.API.Contracts;

public class XmlMwProviderInfo {
    public String ProviderName { get; set; }
    public String ProviderURL { get; set; }
    public XmlMwBlogInfo Blog { get; set; }
    public String UserName { get; set; }
    public String Password { get; set; }
    public Int32 FetchPostCount { get; set; }
}