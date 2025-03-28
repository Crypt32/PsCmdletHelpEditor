using System;

namespace PsCmdletHelpEditor.Core.Models;
/// <summary>
/// Represents command parameter options.
/// <para>This enumeration has a <see cref="FlagsAttribute"/> attribute that allows a bitwise combination of its member values.</para>
/// </summary>
[Flags]
public enum PsCommandParameterOption {
    /// <summary>
    /// None.
    /// </summary>
    None                 = 0,
    /// <summary>
    /// Parameter is mandatory.
    /// </summary>
    Mandatory            = 0x1,
    /// <summary>
    /// Parameter is dynamic.
    /// </summary>
    Dynamic              = 0x2,
    /// <summary>
    /// Accepts pipeline input.
    /// </summary>
    Pipeline             = 0x4,
    /// <summary>
    /// Accepts value from pipeline input by matching property name.
    /// </summary>
    PipelinePropertyName = 0x8,
    /// <summary>
    /// Accepts value from remaining args.
    /// </summary>
    RemainingArgs        = 0x10,
    /// <summary>
    /// Parameter is positional.
    /// </summary>
    Positional           = 0x20,
    /// <summary>
    /// Parameter accepts arrays (collections).
    /// </summary>
    AcceptsArray         = 0x40,
    /// <summary>
    /// Parameter accepts wildcards, such as '*' and '?'.
    /// </summary>
    AcceptsWildcards     = 0x80,
}