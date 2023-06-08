using System;
using CmdletHelpEditor.API.Models;

namespace CmdletHelpEditor.API.Abstractions {
    public interface ISupportInfo {
        PsVersionSupport PsVersion { get; set; }
        WinOsVersionSupport WinOsVersion { get; set; }
        Boolean ADChecked { get; set; }
        Boolean RsatChecked { get; set; }
    }
}