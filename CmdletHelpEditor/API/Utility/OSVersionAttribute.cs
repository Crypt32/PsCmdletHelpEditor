using System;
using PsCmdletHelpEditor.Core.Models;

namespace CmdletHelpEditor.API.Utility;

/// <summary>
/// Represents OS version support attribute applied to properties that are bound to <see cref="WinOsVersionSupport"/> enum.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
class OSVersionAttribute : Attribute {
    public OSVersionAttribute(WinOsVersionSupport name, String displayName) {
        OsVersion = name;
        DisplayName = displayName;
    }

    /// <summary>
    /// Gets OS version enum value.
    /// </summary>
    public WinOsVersionSupport OsVersion { get; }
    /// <summary>
    /// Gets OS version display name.
    /// </summary>
    public String DisplayName { get; }
}
/// <summary>
/// Represents PowerShell version support attribute applied to properties that are bound to <see cref="PsVersionSupport"/> enum.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
class PSVersionAttribute : Attribute {
    public PSVersionAttribute(PsVersionSupport name, String displayName) {
        PsVersion = name;
        DisplayName = displayName;
    }

    /// <summary>
    /// Gets PowerShell version enum value.
    /// </summary>
    public PsVersionSupport PsVersion { get; }
    /// <summary>
    /// Gets PowerShell version display name.
    /// </summary>
    public String DisplayName { get; }
}
