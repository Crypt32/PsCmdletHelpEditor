using System;

namespace PsCmdletHelpEditor.Core;

/// <summary>
/// Represents abstract progress indicator.
/// </summary>
public interface IProgress {
    /// <summary>
    /// Gets or sets progress value. Value shall be between 0 and 100.
    /// </summary>
    Double Progress { get; set; }
}