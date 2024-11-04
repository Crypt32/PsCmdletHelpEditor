using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Services.MAML;
using Unity;

namespace CmdletHelpEditor.API.Tools;
static class FileProcessor {
    public static void SaveProjectFile(ModuleObject tab, String path) {
        using var fs = new FileStream(path, FileMode.Create);
        tab.ProjectPath = path;
        Double oldVersion = tab.FormatVersion;
        // remove read stuff: obsolete cmdlets and parameters
        if (!tab.IsOffline) {
            foreach (CmdletObject cmdlet in tab.Cmdlets.ToArray()) {
                if (cmdlet.GeneralHelp.Status == ItemStatus.Missing) {
                    tab.Cmdlets.Remove(cmdlet);
                } else {
                    foreach (PsCommandParameterVM parameter in cmdlet.Parameters.ToArray().Where(x => x.Status == ItemStatus.Missing)) {
                        cmdlet.Parameters.Remove(parameter);
                    }
                }
            }
        }
        // sort cmdlets by name
        IEnumerable<CmdletObject> cmdlets = tab.Cmdlets
            .OrderBy(x => x.Name)
            .ToList();
        tab.Cmdlets.Clear();
        foreach (CmdletObject cmdlet in cmdlets) {
            tab.Cmdlets.Add(cmdlet);
        }

        tab.FormatVersion = Utils.CurrentFormatVersion;
        var serializer = new XmlSerializer(typeof(ModuleObject));
        try {
            serializer.Serialize(fs, tab);
            tab.ProjectPath = path;
        } catch {
            tab.FormatVersion = oldVersion;
            throw;
        }
    }

    public static async Task PublishHelpFile(this ModuleObject module, String path, IProgressBar pb) {
        if (module.Cmdlets.Count == 0) { return; }
        pb.Start();
        var settings = new XmlWriterSettings {
            Indent = false,
            Async = true,
            NewLineHandling = NewLineHandling.None,
            ConformanceLevel = ConformanceLevel.Document
        };
        try {
            var mamlService = App.Container.Resolve<IMamlService>();
            using var writer = XmlWriter.Create(path, settings);
            await writer.WriteStartDocumentAsync();
            await writer.WriteRawAsync(await mamlService.ExportMamlHelp(module.ToXmlObject().GetCmdlets().ToList(), pb));
        } finally {
            pb.End();
        }
    }
}
