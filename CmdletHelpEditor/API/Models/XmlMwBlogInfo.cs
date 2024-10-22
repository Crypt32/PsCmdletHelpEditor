using System;
using PsCmdletHelpEditor.Core.Models;

namespace CmdletHelpEditor.API.Models;
public class XmlMwBlogInfo : IBlogInfo {
    public String BlogID { get; set; }
    public String BlogName { get; set; }
    public String URL { get; set; }
}