using System;
using System.IO;
using System.Xml.Serialization;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Models.Xml;

namespace PsCmdletHelpEditor.Core.Services;
public interface IProjectService {
    IPsModuleProject ReadProjectFile(String path);
}
public class XmlProjectService {
    public IPsModuleProject ReadProjectFile(String path) {
        XmlPsModuleProject moduleProject;
        XmlAttributeOverrides? overrides = XmlFormatConverter.GetOverrides(path, out Double version);
        using var fs = new FileStream(path, FileMode.Open);
        XmlSerializer serializer = overrides == null
            ? new XmlSerializer(typeof(XmlPsModuleProject))
            : new XmlSerializer(typeof(XmlPsModuleProject), overrides);
        moduleProject = (XmlPsModuleProject)serializer.Deserialize(fs);
        moduleProject.ProjectPath = path;
        moduleProject.FormatVersion = version;

        return moduleProject;
    }
}