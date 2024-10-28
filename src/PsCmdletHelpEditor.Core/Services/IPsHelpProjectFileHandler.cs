using System;
using PsCmdletHelpEditor.Core.Models;

namespace PsCmdletHelpEditor.Core.Services;

public interface IPsHelpProjectFileHandler {
    /// <summary>
    /// Gets current project file schema version.
    /// </summary>
    Double CurrentFormatVersion { get; }

    /// <summary>
    /// Reads help project from a file.
    /// </summary>
    /// <param name="path">Path to <c>.pshproj</c> file.</param>
    /// <returns>Help project object.</returns>
    IPsModuleProject ReadProjectFile(String path);
    /// <summary>
    /// Writes help project to a file.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="path"></param>
    void SaveProjectFile(IPsModuleProject project, String path);
}