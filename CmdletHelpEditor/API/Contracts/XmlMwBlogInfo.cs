using System;
using CmdletHelpEditor.API.Abstractions;

namespace CmdletHelpEditor.API.Contracts;
public class XmlMwBlogInfo : IBlogInfo {
    public String BlogID { get; set; }
    public String BlogName { get; set; }
    public String URL { get; set; }
}