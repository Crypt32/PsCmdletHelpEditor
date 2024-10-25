using System;

namespace PsCmdletHelpEditor.Core.Models;

public interface IPsCommandSupportInfo {
    PsVersionSupport PsVersion { get; }
    WinOsVersionSupport WinOsVersion { get; }
    Boolean RequiresAD { get; }
    Boolean RequiresRSAT { get; }
}