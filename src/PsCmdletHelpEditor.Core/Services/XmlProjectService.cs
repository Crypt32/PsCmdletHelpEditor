using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Models.Xml;
using System;
using System.IO;
using System.Xml.Serialization;

namespace PsCmdletHelpEditor.Core.Services;
public interface IProjectService {
    IPsModule ReadProjectFile(String path);
}
public class XmlProjectService {
    public IPsModule ReadProjectFile(String path) {
        XmlPsModule module;
        XmlAttributeOverrides? overrides = XmlFormatConverter.GetOverrides(path, out Double version);
        using var fs = new FileStream(path, FileMode.Open);
        XmlSerializer serializer = overrides == null
            ? new XmlSerializer(typeof(XmlPsModule))
            : new XmlSerializer(typeof(XmlPsModule), overrides);
        module = (XmlPsModule)serializer.Deserialize(fs);
        module.ProjectPath = path;
        module.FormatVersion = version;

        return module;
    }
}