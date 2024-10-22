using System;
using System.Collections.Generic;

namespace PsCmdletHelpEditor.Core.Models;

public interface IPsCommandInfo {
    /// <summary>
    /// Gets cmdlet name.
    /// </summary>
    String Name { get; }
    /// <summary>
    /// Gets cmdlet verb.
    /// </summary>
    String Verb { get; }
    /// <summary>
    /// Gets cmdlet noun.
    /// </summary>
    String Noun { get; }
    /// <summary>
    /// Gets a list of cmdlet syntaxes, one per parameter set.
    /// </summary>
    List<String> Syntax { get; }
    /// <summary>
    /// Gets optional help header.
    /// </summary>
    String? ExtraHeader { get; }
    /// <summary>
    /// Gets optional help footer.
    /// </summary>
    String? ExtraFooter { get; }

    /// <summary>
    /// Gets cmdlet description.
    /// </summary>
    /// <returns>Cmdlet description.</returns>
    IPsCommandGeneralDescription GetDescription();
    /// <summary>
    /// Gets optional command support info.
    /// </summary>
    /// <returns>Command support info.</returns>
    IPsCommandSupportInfo? GetSupportInfo();
    /// <summary>
    /// Gets a collection of cmdlet parameter sets.
    /// </summary>
    /// <returns>A collection of cmdlet parameter sets.</returns>
    IReadOnlyList<IPsCommandParameterSetInfo> GetParameterSets();
    /// <summary>
    /// Gets a collection of cmdlet parameters.
    /// </summary>
    /// <returns>A collection of cmdlet parameters.</returns>
    IReadOnlyList<IPsCommandParameterDescription> GetParameters();
    /// <summary>
    /// Gets a collection of cmdlet examples.
    /// </summary>
    /// <returns>A collection of cmdlet examples.</returns>
    IReadOnlyList<IPsCommandExample> GetExamples();
    /// <summary>
    /// Gets a collection of cmdlet related links.
    /// </summary>
    /// <returns>A collection of cmdlet related links.</returns>
    IReadOnlyList<IPsCommandRelatedLink> GetRelatedLinks();
}