using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;

namespace CmdletHelpEditor.API.Tools {
    static class FileProcessor {
        public static ModuleObject ReadProjectFile(String path) {
            FileStream fs = null;
            ModuleObject module;
            try {
                Double version;
                XmlAttributeOverrides overrides = XmlFormatConverter.GetOverrides(path, out version);
                fs = new FileStream(path, FileMode.Open);
                XmlSerializer serializer = overrides == null
                    ? new XmlSerializer(typeof(ModuleObject))
                    : new XmlSerializer(typeof(ModuleObject), overrides);
                module = (ModuleObject)serializer.Deserialize(fs);
                module.ProjectPath = path;
                module.FormatVersion = version;
            } finally {
                fs?.Close();
            }
            return module;
        }
        public static void SaveProjectFile(ClosableModuleItem tab, String path) {
            FileStream fs = new FileStream(path, FileMode.Create);
            tab.Module.ProjectPath = path;
            Double oldVersion = tab.Module.FormatVersion;
            // remove read stuff: obsolete cmdlets and parameters
            if (!tab.Module.IsOffline) {
                foreach (CmdletObject cmdlet in tab.Module.Cmdlets.ToArray()) {
                    if (cmdlet.GeneralHelp.Status == ItemStatus.Missing) {
                        tab.Module.Cmdlets.Remove(cmdlet);
                    } else {
                        foreach (ParameterDescription parameter in cmdlet.Parameters.ToArray().Where(x => x.Status == ItemStatus.Missing)) {
                            cmdlet.Parameters.Remove(parameter);
                        }
                    }
                }
            }
            // sort cmdlets by name
            IEnumerable<CmdletObject> cmdlets = tab.Module.Cmdlets
                .OrderBy(x => x.Name)
                .ToList();
            tab.Module.Cmdlets.Clear();
            foreach (CmdletObject cmdlet in cmdlets) {
                tab.Module.Cmdlets.Add(cmdlet);
            }

            tab.Module.FormatVersion = Utils.CurrentFormatVersion;
            XmlSerializer serializer = new XmlSerializer(typeof(ModuleObject));
            try {
                serializer.Serialize(fs, tab.Module);
                tab.Module.ProjectPath = path;
                tab.IsSaved = true;
            } catch {
                tab.Module.FormatVersion = oldVersion;
                throw;
            } finally {
                fs.Close();
            }
        }
        public static Boolean FindModule(String moduleName) {
            String modulePaths = Environment.GetEnvironmentVariable("PSModulePath", EnvironmentVariableTarget.Process);
            if (String.IsNullOrEmpty(modulePaths)) { return false; }
            return modulePaths
                .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(path => new DirectoryInfo(path))
                .Where(x => x.Exists)
                .Any(dir => dir.EnumerateDirectories(moduleName).Any());
        }

        public static async Task PublishHelpFile(this ModuleObject module, String path, IProgressBar pb) {
            XmlWriter writer = null;
            if (module.Cmdlets.Count == 0) { return; }
            pb.Start();
            XmlWriterSettings settings = new XmlWriterSettings {
                Indent = false,
                NewLineHandling = NewLineHandling.None,
                ConformanceLevel = ConformanceLevel.Document
            };
            try {
                writer = XmlWriter.Create(path, settings);
                await writer.WriteStartDocumentAsync();
                await writer.WriteRawAsync(await XmlProcessor.XmlGenerateHelp(module.Cmdlets, pb, module.IsOffline));
            } finally {
                pb.End();
                writer?.Close();
            }
        }
    }
}
