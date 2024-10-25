using System;

namespace PsCmdletHelpEditor.Core.Models;

public class XmlRpcBlogInfo : IBlogInfo {
    public String BlogID { get; set; }
    public String BlogName { get; set; }
    public String URL { get; set; }
}