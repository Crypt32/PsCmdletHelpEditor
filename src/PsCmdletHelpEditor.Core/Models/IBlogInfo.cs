using System;

namespace PsCmdletHelpEditor.Core.Models;

public interface IBlogInfo {
    String BlogID { get; set; }
    String BlogName { get; set; }
    String URL { get; set; }
}