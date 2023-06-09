using System;

namespace CmdletHelpEditor.API.Abstractions;

public interface IBlogInfo {
    String BlogID { get; set; }
    String BlogName { get; set; }
    String URL { get; set; }
}