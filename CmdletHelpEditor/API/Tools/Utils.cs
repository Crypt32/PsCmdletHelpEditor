using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.Properties;
using PsCmdletHelpEditor.Core.Services;
using Unity;

namespace CmdletHelpEditor.API.Tools;
static class Utils {
    public static Double CurrentFormatVersion => 1.1;

    public static IEnumerable<Double> SupportedFormatVersions => [0, 1.0, 1.1];
    public static IEnumerable<ProviderInformation> EnumProviders() {
        return new ObservableCollection<ProviderInformation> {
            new() {
                      ProviderName = "CodePlex",
                      ProviderURL = "https://www.codeplex.com/site/metaweblog"
                  },
            new() {
                      ProviderName = "Custom"
                  }
        };
    }
    public static IReadOnlyList<String> GetCommandTypes() {
        var psProcessor = App.Container.Resolve<IPowerShellProcessor>();
        List<String> commandTypes = [];
        if (Settings.Default.FunctionChecked) { commandTypes.Add("Function"); }
        if (Settings.Default.FilterChecked) { commandTypes.Add("Filter"); }
        if (Settings.Default.CmdletChecked) { commandTypes.Add("Cmdlet"); }
        if (Settings.Default.ExternalScriptChecked) { commandTypes.Add("ExternalScript"); }
        if (Settings.Default.ScriptChecked) { commandTypes.Add("Script"); }
        if (psProcessor.PsVersion >= 3 && Settings.Default.WorkflowChecked) { commandTypes.Add("Workflow"); }
        if (psProcessor.PsVersion >= 4 && Settings.Default.ApplicationChecked) { commandTypes.Add("Application"); }

        return commandTypes;
    }
}
