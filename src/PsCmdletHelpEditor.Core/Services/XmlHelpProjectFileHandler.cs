using System;
using System.IO;
using System.Xml.Serialization;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Models.Xml;

namespace PsCmdletHelpEditor.Core.Services;

/// <summary>
/// Represents XML-based help project file handler.
/// </summary>
public class XmlHelpProjectFileHandler : IPsHelpProjectFileHandler {
    public Double CurrentFormatVersion => 1.1;

    /// <inheritdoc />
    public IPsModuleProject ReadProjectFile(String path) {
        XmlAttributeOverrides overrides = XmlFormatConverter.GetOverrides(path, out Double version);
        using var fs = new FileStream(path, FileMode.Open);
        XmlSerializer serializer = overrides == null
            ? new XmlSerializer(typeof(XmlPsModuleProject))
            : new XmlSerializer(typeof(XmlPsModuleProject), overrides);
        var module = (XmlPsModuleProject)serializer.Deserialize(fs);
        module.ProjectPath = path;
        module.FormatVersion = version;

        return module;
    }
    /// <inheritdoc />
    public void SaveProjectFile(IPsModuleProject project, String path) {
        var xmlObject = ToXmlPsModuleProject(project);
        using var fs = new FileStream(path, FileMode.Create);
        project.ProjectPath = path;

        // backup project schema version into local variable
        Double oldVersion = project.FormatVersion;
        // set schema version to latest.
        project.FormatVersion = CurrentFormatVersion;
        var serializer = new XmlSerializer(typeof(XmlPsModuleProject));
        try {
            serializer.Serialize(fs, project);
            project.ProjectPath = path;
        } catch {
            // restore schema version if project save failed.
            project.FormatVersion = oldVersion;
            throw;
        }
    }
    static XmlPsModuleProject ToXmlPsModuleProject(IPsModuleProject project) {
        var retValue = new XmlPsModuleProject {
            FormatVersion = project.FormatVersion,
            Name = project.Name

        };

        return retValue;
    }
}
