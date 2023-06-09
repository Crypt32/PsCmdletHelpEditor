using System;
using System.Collections.Generic;

namespace CmdletHelpEditor.API.Abstractions;

public interface IParameterSetInfo {
    String Name { get; set; }
    List<String> Parameters { get; set; }
}