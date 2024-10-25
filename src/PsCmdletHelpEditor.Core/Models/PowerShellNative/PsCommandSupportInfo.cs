using System;

namespace PsCmdletHelpEditor.Core.Models.PowerShellNative;
class PsCommandSupportInfo : IPsCommandSupportInfo {
    public PsVersionSupport PsVersion { get; private set; }
    public WinOsVersionSupport WinOsVersion { get; private set; }
    public Boolean RequiresAD { get; private set; }
    public Boolean RequiresRSAT { get; private set; }

    public static PsCommandSupportInfo CreateDefault() {
        return new PsCommandSupportInfo();
    }
    public static PsCommandSupportInfo FromSupportInfo(IPsCommandSupportInfo supportInfo) {
        return new PsCommandSupportInfo {
            PsVersion = supportInfo.PsVersion,
            WinOsVersion = supportInfo.WinOsVersion,
            RequiresAD = supportInfo.RequiresAD,
            RequiresRSAT = supportInfo.RequiresRSAT
        };
    }
}
