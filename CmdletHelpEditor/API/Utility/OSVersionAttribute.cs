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
