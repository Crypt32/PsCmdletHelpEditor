using System;

namespace PsCmdletHelpEditor.Core.Models;

public class BlogInfo : IBlogInfo {
    public String BlogID { get; set; }
    public String BlogName { get; set; }
    public String URL { get; set; }
}