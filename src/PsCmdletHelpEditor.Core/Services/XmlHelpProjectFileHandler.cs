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
        module.FormatVersion = version;

        return module;
    }
    /// <inheritdoc />
    public void SaveProjectFile(IPsModuleProject project, String path) {
        // TODO: for now, argument must be XML object, although parameter accepts interface.
        var xmlObject = (XmlPsModuleProject)project;
        using var fs = new FileStream(path, FileMode.Create);

        // backup project schema version into local variable
        Double oldVersion = project.FormatVersion;
        // set schema version to latest.
        xmlObject.FormatVersion = CurrentFormatVersion;
        var serializer = new XmlSerializer(typeof(XmlPsModuleProject));
        try {
            serializer.Serialize(fs, project);
        } catch {
            // restore schema version if project save failed.
            xmlObject.FormatVersion = oldVersion;
            throw;
        }
    }
}
