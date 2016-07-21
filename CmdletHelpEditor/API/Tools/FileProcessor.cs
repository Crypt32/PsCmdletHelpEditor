using CmdletHelpEditor.API.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;

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
				if (fs != null) { fs.Close(); }
			}
			return module;
		}
		public static void SaveProjectFile(ClosableModuleItem tab, String path) {
			FileStream fs = new FileStream(path, FileMode.Create);
			tab.Module.ProjectPath = path;
			Double oldVersion = tab.Module.FormatVersion;
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

		public static async void PublishHelpFile(String path, ModuleObject module, ProgressBar pb) {
			XmlWriter writer = null;
			StringBuilder SB = new StringBuilder();
			if (module.Cmdlets.Count == 0) { return; }
			pb.Value = 0;
			pb.Visibility = Visibility.Visible;
			XmlWriterSettings settings = new XmlWriterSettings {
				Indent = true,
				IndentChars = ("	"),
				NewLineHandling = NewLineHandling.None,
				ConformanceLevel = ConformanceLevel.Document
			};
			try {
				writer = XmlWriter.Create(path, settings);
				writer.WriteStartDocument();
				await XmlProcessor.XmlGenerateHelp(SB, module.Cmdlets, pb, module.IsOffline);
				writer.WriteRaw(SB.ToString());
			} finally {
				pb.Visibility = Visibility.Collapsed;
				if (writer != null) { writer.Close(); }
			}
		}
	}
}
