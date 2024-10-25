using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using PsCmdletHelpEditor.Core.Models.Xml;

namespace PsCmdletHelpEditor.Core.Services;

static class XmlFormatConverter {
    public static XmlAttributeOverrides? GetOverrides(String path, out Double fVersion) {
        fVersion = GetFormatVersion(path);
        if (fVersion.Equals(0.0)) {
            return getV0();
        }
        return null;
    }
    // version checker
    static Double GetFormatVersion(String file) {
        XmlDocument doc = new XmlDocument();
        doc.Load(file);
        XmlNode? selectSingleNode = doc.SelectSingleNode("ModuleObject");
        if (selectSingleNode == null) {
            throw new Exception("Root XML element not found.");
        }
        if (selectSingleNode.Attributes?["fVersion"] != null) {
            if (Double.TryParse(selectSingleNode.Attributes["fVersion"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out Double result)) {
                return result;
            }
        }

        return 0;
    }
    static XmlAttributeOverrides getV0() {
        XmlAttributes legacyElements;
        var tbo = new XmlAttributeOverrides();
        foreach (String prop in new[] { "ModuleClass", "UseSupports" }) {
            legacyElements = new XmlAttributes();
            legacyElements.XmlElements.Add(new XmlElementAttribute(prop));
            tbo.Add(typeof(XmlPsModuleProject), prop, legacyElements);
        }
        foreach (String prop in new[] { "Verb", "Noun" }) {
            legacyElements = new XmlAttributes();
            legacyElements.XmlElements.Add(new XmlElementAttribute(prop));
            tbo.Add(typeof(XmlPsCommand), prop, legacyElements);
        }
        foreach (String prop in new[] { "Type", "AcceptsArray", "Mandatory", "Dynamic", "RemainingArgs", "Pipeline", "PipelinePropertyName", "Positional", "Position", "Globbing" }) {
            legacyElements = new XmlAttributes();
            legacyElements.XmlElements.Add(new XmlElementAttribute(prop));
            tbo.Add(typeof(XmlPsCommandParameterDescription), prop, legacyElements);
        }
        String[] strs = [
            "ADChecked","RsatChecked","Ps2Checked","Ps3Checked","Ps4Checked","Ps5Checked","WinXpChecked","WinVistaChecked","Win7Checked",
            "Win8Checked","Win81Checked","Win2003StdChecked","Win2003EEChecked","Win2003DCChecked","Win2008StdChecked","Win2008EEChecked",
            "Win2008DCChecked","Win2008R2StdChecked","Win2008R2EEChecked","Win2008R2DCChecked","Win2012StdChecked","Win2012DCChecked",
            "Win2012R2StdChecked","Win2012R2DCChecked"
        ];
        foreach (String prop in strs) {
            legacyElements = new XmlAttributes();
            legacyElements.XmlElements.Add(new XmlElementAttribute(prop));
            tbo.Add(typeof(XmlPsCommandSupportInfo), prop, legacyElements);
        }
        return tbo;
    }
}