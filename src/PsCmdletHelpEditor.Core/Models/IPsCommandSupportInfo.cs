using System;

namespace PsCmdletHelpEditor.Core.Models;

public interface IPsCommandSupportInfo {
    PsVersionSupport PsVersion { get; set; }
    WinOsVersionSupport WinOsVersion { get; set; }
    Boolean ADChecked { get; set; }
    Boolean RsatChecked { get; set; }
}